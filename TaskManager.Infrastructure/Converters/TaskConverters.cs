using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManager.Core.Enums;

namespace TaskManager.Infrastructure.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                Priority.High => Brushes.OrangeRed,
                Priority.Medium => Brushes.Orange,
                Priority.Low => Brushes.LightGreen,
                _ => Brushes.LightGray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                TasksStatus.Completed => Brushes.Green,
                TasksStatus.InProgress => Brushes.Blue,
                TasksStatus.OnHold => Brushes.Gray,
                _ => Brushes.Black
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DeadlineToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isNear && isNear ? Brushes.Red : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
