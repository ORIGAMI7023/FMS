using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FMS.Mobile.Controls
{
    public partial class CustomCalendarView : ContentView
    {
        public static readonly BindableProperty SelectedDateProperty =
            BindableProperty.Create(nameof(SelectedDate),
                                    typeof(DateTime),
                                    typeof(CustomCalendarView),
                                    DateTime.Today, BindingMode.TwoWay);

        public static readonly BindableProperty DailyMapProperty =
            BindableProperty.Create(
                nameof(DailyMap),
                typeof(Dictionary<DateOnly, decimal>),
                typeof(CustomCalendarView),
                new Dictionary<DateOnly, decimal>(),
                propertyChanged: OnDailyMapChanged);

        private bool _autoSelect;//用于区分当前是否正在切换月份以在BuildCalendar方法中自动选择日期


        //当前选中的日期
        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public Dictionary<DateOnly, decimal> DailyMap
        {
            get => (Dictionary<DateOnly, decimal>)GetValue(DailyMapProperty);
            set => SetValue(DailyMapProperty, value);
        }

        private DateTime _displayMonth;//当前选中年月的第一天？
        private readonly List<Button> _buttonPool = new();
        private readonly Dictionary<DateTime, Button> _dateButtons = new();

        //构造函数
        public CustomCalendarView()
        {
            InitializeComponent();
            _displayMonth = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
            BuildCalendar();
        }

        /// <summary>
        /// 上一个月份按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPrevMonthClicked(object sender, EventArgs e)
        {
            _displayMonth = _displayMonth.AddMonths(-1);
            BuildCalendar();
        }
        /// <summary>
        /// 下一个月份按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            _displayMonth = _displayMonth.AddMonths(1);
            BuildCalendar();
        }

        /// <summary>
        /// 单击年月标签，弹出年份选择框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnMonthLabelTapped(object sender, EventArgs e)
        {
            var yearList = new List<string>();
            int currentYear = DateTime.Now.Year;//获取当前年份
            for (int y = 2024; y <= currentYear; y++)
                yearList.Add(y.ToString());

            string selected = await Shell.Current.DisplayActionSheet("选择年份", "取消", null, yearList.ToArray());

            if (int.TryParse(selected, out int year) && (year != currentYear))//修改年份时，重绘界面
            {
                _displayMonth = new DateTime(year, _displayMonth.Month, 1);
                BuildCalendar();
            }
        }

        /// <summary>
        /// 在页面上绘制控件中的日历视图
        /// </summary>
        private void BuildCalendar()
        {
            MonthLabel.Text = _displayMonth.ToString("yyyy年M月", CultureInfo.CurrentCulture);
            DateGrid.Children.Clear();
            _dateButtons.Clear();

            //绘制六行七列grid
            if (DateGrid.RowDefinitions.Count == 0)
            {
                for (int i = 0; i < 6; i++)
                    DateGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            }
            if (DateGrid.ColumnDefinitions.Count == 0)
            {
                for (int i = 0; i < 7; i++)
                    DateGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            }

            DateTime firstDay = new DateTime(_displayMonth.Year, _displayMonth.Month, 1);
            int daysCount = DateTime.DaysInMonth(_displayMonth.Year, _displayMonth.Month);//当前月份的日数
            int col = (int)firstDay.DayOfWeek;//获取每个月1号所在的列
            int row = 0;//每个月1号放在第1行

            int poolIndex = 0;//计算已绘制的button数量

            for (int i = 1; i <= daysCount; i++)//遍历绘制每天的按钮
            {
                DateTime currentDate = new DateTime(_displayMonth.Year, _displayMonth.Month, i);//获取当前正在绘制的日期
                Button btn;

                if (poolIndex < _buttonPool.Count)//如果已经有绘制好的按钮，则直接使用
                {
                    btn = _buttonPool[poolIndex];
                }
                else//否侧重新绘制按钮
                {
                    btn = new Button
                    {
                        FontSize = 18,
                        Padding = 0,
                        Margin = new Thickness(6),
                        CornerRadius = 9999,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 40,
                        HeightRequest = 40
                    };
                    btn.Clicked += OnDateClicked;
                    _buttonPool.Add(btn);//在已绘制列表中加入刚创建的button
                }

                btn.Text = i.ToString();//设置按钮文本为当前日期的日
                btn.CommandParameter = currentDate;//设置按钮的命令参数为当前日期

                //在视图中添加当前日期的按钮
                Grid.SetRow(btn, row);
                Grid.SetColumn(btn, col);
                DateGrid.Children.Add(btn);
                _dateButtons[currentDate] = btn;

                col++;
                if (col > 6)
                {
                    col = 0;
                    row++;
                }

                poolIndex++;
            }


            if (_autoSelect) // 仅当不是用户操作时，自动选中
            {
                DateTime newDate = (DateTime.Today.Month == _displayMonth.Month && DateTime.Today.Year == _displayMonth.Year)
                ? DateTime.Today
                : new DateTime(_displayMonth.Year, _displayMonth.Month, 1);

                if (SelectedDate != newDate)
                    SelectedDate = newDate;
            }
            UpdateSelectionVisual();
        }

        /// <summary>
        /// 绘制选中日期的视觉效果，包含背景色和文本颜色的设置
        /// </summary>
        private void UpdateSelectionVisual()
        {
            _autoSelect = true; // 重置标志位
            foreach (var kvp in _dateButtons)//遍历已添加的按钮
            {
                Button btn = kvp.Value;
                DateTime date = kvp.Key;
                if (date.Date == SelectedDate.Date)//如果当前遍历的日期等于选中日期，则设置选中样式为选中
                {
                    btn.BackgroundColor = (date.Date == DateTime.Today.Date) ? //如果选中日期是今天，则使用主题色，否则使用浅蓝色
                        Application.Current.Resources["Primary"] as Color : Colors.LightBlue;

                    btn.TextColor = Colors.White;
                }
                else if (date.Date == DateTime.Today.Date)
                {
                    btn.BackgroundColor = Colors.Transparent;
                    btn.TextColor = Colors.Red;
                }
                else
                {
                    btn.BackgroundColor = Colors.Transparent;
                    btn.TextColor = Colors.Black;
                }
            }
        }

        /// <summary>
        /// 单击某个日期按钮时触发，更新选中日期并刷新视觉效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDateClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is DateTime dt)
            {
                _autoSelect = false; // 按下的是日期按钮，禁止自动选择
                if (SelectedDate != dt)//仅在选中日期发生变化时更新界面
                {
                    SelectedDate = dt;
                    UpdateSelectionVisual();
                }
            }
        }

        private static void OnDailyMapChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendarView calendar)
            {
                calendar.BuildCalendar(); // 刷新 UI（或你自己的刷新逻辑）
            }
        }
    }
}
