// 文件：Controls/CustomCalendarView.xaml.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

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

        private void BuildCalendar()
        {
            MonthLabel.Text = _displayMonth.ToString("yyyy年M月", CultureInfo.CurrentCulture);
            DateGrid.Children.Clear();
            DateGrid.RowDefinitions.Clear();
            DateGrid.ColumnDefinitions.Clear();
            _dateButtons.Clear();

            for (int i = 0; i < 7; i++)
                DateGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            for (int i = 0; i < 6; i++)
                DateGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));

            DateTime firstDay = new DateTime(_displayMonth.Year, _displayMonth.Month, 1);
            int days = DateTime.DaysInMonth(_displayMonth.Year, _displayMonth.Month);
            int col = (int)firstDay.DayOfWeek;
            int row = 0;

            for (int i = 1; i <= days; i++)
            {
                DateTime currentDate = new DateTime(_displayMonth.Year, _displayMonth.Month, i);

                var btn = new Button
                {
                    Text = i.ToString(),
                    FontSize = 14,
                    Padding = new Thickness(0),
                    Margin = new Thickness(2),
                    BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Black,
                    CommandParameter = currentDate
                };

                btn.Clicked += (s, e) =>
                {
                    SelectedDate = currentDate;
                    UpdateSelectionVisual();
                };

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
            }

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
                    btn.BackgroundColor = Colors.LightBlue;
                    btn.TextColor = Colors.White;
                }
                else
                {
                    btn.BackgroundColor = Colors.Transparent;
                    btn.TextColor = Colors.Black;
                }
            }
        }
    }
}
