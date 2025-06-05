using System.Globalization;
using System.Windows.Data;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Converters
{
    public class SelectedTaskToIsSelectedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return false;
            if (values[0] is Tasks selectedTask && values[1] is Tasks currentTask)
                return selectedTask?.Id == currentTask?.Id;
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
