// 文件：Controls/CustomCalendarView.xaml.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FMS.Mobile.Controls
{
    public partial class CustomCalendarView : ContentView
    {
        public static readonly BindableProperty SelectedDateProperty =
            BindableProperty.Create(nameof(SelectedDate), typeof(DateTime), typeof(CustomCalendarView), DateTime.Today, BindingMode.TwoWay);

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        private DateTime _displayMonth;
        private readonly List<Button> _buttonPool = new();
        private readonly Dictionary<DateTime, Button> _dateButtons = new();

        public CustomCalendarView()
        {
            InitializeComponent();
            _displayMonth = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
            BuildCalendar();
        }

        private void OnPrevMonthClicked(object sender, EventArgs e)
        {
            _displayMonth = _displayMonth.AddMonths(-1);
            BuildCalendar();
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            _displayMonth = _displayMonth.AddMonths(1);
            BuildCalendar();
        }

        private async void OnMonthLabelTapped(object sender, EventArgs e)
        {
            var yearList = new List<string>();
            int currentYear = DateTime.Now.Year;
            for (int y = currentYear - 10; y <= currentYear + 10; y++)
                yearList.Add(y.ToString());

            string selected = await Shell.Current.DisplayActionSheet("选择年份", "取消", null, yearList.ToArray());
            if (int.TryParse(selected, out int year))
            {
                _displayMonth = new DateTime(year, _displayMonth.Month, 1);
                BuildCalendar();
            }
        }

        private void BuildCalendar()
        {
            MonthLabel.Text = _displayMonth.ToString("yyyy年M月", CultureInfo.CurrentCulture);
            DateGrid.Children.Clear();
            _dateButtons.Clear();

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
            int days = DateTime.DaysInMonth(_displayMonth.Year, _displayMonth.Month);
            int col = (int)firstDay.DayOfWeek;
            int row = 0;

            int poolIndex = 0;

            for (int i = 1; i <= days; i++)
            {
                DateTime currentDate = new DateTime(_displayMonth.Year, _displayMonth.Month, i);
                Button btn;

                if (poolIndex < _buttonPool.Count)
                {
                    btn = _buttonPool[poolIndex];
                }
                else
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
                    _buttonPool.Add(btn);
                }

                btn.Text = i.ToString();
                btn.CommandParameter = currentDate;

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

            SelectedDate = (DateTime.Today.Month == _displayMonth.Month && DateTime.Today.Year == _displayMonth.Year)
                ? DateTime.Today
                : new DateTime(_displayMonth.Year, _displayMonth.Month, 1);

            UpdateSelectionVisual();
        }

        private void UpdateSelectionVisual()
        {
            foreach (var kvp in _dateButtons)
            {
                var btn = kvp.Value;
                var date = kvp.Key;
                if (date.Date == SelectedDate.Date)
                {
                    btn.BackgroundColor = (date.Date == DateTime.Today.Date) ? Application.Current.Resources["Primary"] as Color : Colors.LightBlue;
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

        private void OnDateClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is DateTime dt)
            {
                SelectedDate = dt;
                UpdateSelectionVisual();
            }
        }
    }
}
