using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp1
{
    public class UploadProgressTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double progress && values[1] is bool isUploading)
            {
                return isUploading ? $"{progress:0.00}% Completed" : "Ready";
            }

            return "Ready";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
