using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Klient
{
    /// <summary>
    /// Klasa pomocnicza implementujaca konwertery, uzywane do konwersji typow zbindowanych wlasnosci w kontrolkach wedlug wzorca MVVM
    /// </summary>
    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
