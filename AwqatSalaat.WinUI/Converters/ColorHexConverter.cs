using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace AwqatSalaat.WinUI.Converters
{
    internal class ColorHexConverter : IValueConverter
    {
        private static Color HexToColor(string hex)
        {
            // Init using magenta color
            byte[] bytes = new byte[] { 255, 255, 0, 255 };

            try
            {
                var chars = hex.TrimStart('#').ToCharArray();
                var span = new ReadOnlySpan<char>(chars);
                bool hasAlpha = span.Length == 8;
                int numDigit = span.Length == 3 ? 1 : 2;
                int indexStart = hasAlpha ? 0 : 1;

                for (int i = 0; i < span.Length; i += numDigit)
                {
                    var slice = span.Slice(i, numDigit);
                    var b = numDigit == 2
                        ? byte.Parse(slice, System.Globalization.NumberStyles.HexNumber)
                        : byte.Parse(string.Concat(slice, slice), System.Globalization.NumberStyles.HexNumber);
                    bytes[indexStart + (i / numDigit)] = b;
                }
            }
            catch(Exception ex)
            {
#if DEBUG
                throw;
#endif
            }

            return new Color()
            {
                A = bytes[0],
                R = bytes[1],
                G = bytes[2],
                B = bytes[3],
            };
        }

        private static string ColorToHex(Color color) => $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

        private static object ConvertImpl(object value, Type targetType, object parameter, string language)
        {
            if (value is string hex)
            {
                var color = string.IsNullOrEmpty(hex) ? Color.FromArgb(0, 0, 0, 0) : HexToColor(hex);

                if (targetType == typeof(Color))
                {
                    return color;
                }
                else if (targetType == typeof(Brush))
                {
                    return new SolidColorBrush(color);
                }
            }
            else if (value is Color color && targetType == typeof(string))
            {
                return ColorToHex(color);
            }

            return null;

        }

        public object Convert(object value, Type targetType, object parameter, string language)
            => ConvertImpl(value, targetType, parameter, language);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => ConvertImpl(value, targetType, parameter, language);
    }
}
