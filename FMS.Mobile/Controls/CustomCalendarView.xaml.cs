using System;
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

        private void BuildCalendar()
        {
            MonthLabel.Text = _displayMonth.ToString("yyyy年M月", CultureInfo.CurrentCulture);
            DateGrid.Children.Clear();
            DateGrid.RowDefinitions.Clear();
            DateGrid.ColumnDefinitions.Clear();

            // 7 列
            for (int i = 0; i < 7; i++)
                DateGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            // 6 行：避免错漏，哪怕有时只用 4~5 行
            for (int i = 0; i < 6; i++)
                DateGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));

            // 当前月信息
            DateTime firstDay = new DateTime(_displayMonth.Year, _displayMonth.Month, 1);
            int days = DateTime.DaysInMonth(_displayMonth.Year, _displayMonth.Month);
            int col = (int)firstDay.DayOfWeek;
            int row = 0;

            for (int i = 1; i <= days; i++)
            {
                var btn = new Button
                {
                    Text = i.ToString(),
                    FontSize = 14,
                    Padding = new Thickness(0),
                    Margin = new Thickness(2),
                    BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Black  
                };

                if (SelectedDate.Date == new DateTime(_displayMonth.Year, _displayMonth.Month, i))
                {
                    btn.BackgroundColor = Colors.LightBlue;
                    btn.TextColor = Colors.White; 
                }


                int day = i;
                btn.Clicked += (s, e) =>
                {
                    SelectedDate = new DateTime(_displayMonth.Year, _displayMonth.Month, day);
                    BuildCalendar();
                };

                Grid.SetRow(btn, row);
                Grid.SetColumn(btn, col);
                DateGrid.Children.Add(btn);

                col++;
                if (col > 6)
                {
                    col = 0;
                    row++;
                }
            }

        }

        private void OnDateClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is DateTime dt)
            {
                SelectedDate = dt;
                BuildCalendar();
            }
        }
    }
}
