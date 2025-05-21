using System;
using System.Globalization;
using System.Windows.Data;

namespace FMS.Client.Views
{
    public class DuplicateDoctorHider : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null) return values[0];
            string current = values[0].ToString();
            string previous = values[1].ToString();
            return current == previous ? string.Empty : current;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}