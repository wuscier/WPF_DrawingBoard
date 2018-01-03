using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF_DrawingBoard.Converters
{
    public  class ColorToBrushConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            if (value != null)
            {
                Color color = (Color) value;
                brush = new SolidColorBrush(color);
                return brush;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
