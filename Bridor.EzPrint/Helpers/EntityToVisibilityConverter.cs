using System;
using System.Windows;
using System.Windows.Data;

namespace Bridor.EzPrint.Helpers
{
    public class EntityToVisibilityConverter : IValueConverter
    {
        #region -  IValueConverter Members  -

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (targetType != typeof(System.Windows.Visibility)) {
                throw new InvalidOperationException("The target type must be System.Windows.Visibility");
            }
            return (value == null) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            // No need to convert back
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}
