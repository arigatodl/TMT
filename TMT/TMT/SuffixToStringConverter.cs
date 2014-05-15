using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TMT
{
    class SuffixToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string returnValue = "";

            if (value != null)
            {
                List<string> suffixes = (List<string>) value;
                foreach(string suffix in suffixes)
                {
                    returnValue += suffix + "+";
                }
                return returnValue;
            }
            else
            {
                return returnValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
