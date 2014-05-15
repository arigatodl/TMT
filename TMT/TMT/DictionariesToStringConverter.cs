using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TMT
{
    using TMT.Model;
    using System.Collections.ObjectModel;

    class DictionariesToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string returnValue = "";
            Console.WriteLine("Entered");
            if (value != null)
            {
                ObservableCollection<Dictionary> dictionaries = (ObservableCollection<Dictionary>)value;
                foreach(Dictionary dictionary in dictionaries)
                {
                    Console.WriteLine(dictionary.SLWord + " " + dictionary.TLWord);
                    returnValue += dictionary.TLWord + " ";
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
