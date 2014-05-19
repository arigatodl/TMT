using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ViewModel
{
    using System.Collections.ObjectModel;
    using TMT.Model;
    using System.ComponentModel;

    class TMDictionaryViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Dictionary> dictionaries;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        public TMDictionaryViewModel()
        {
            dictionaries = new AsyncObservableCollection<Dictionary>();
        }

        public ObservableCollection<Dictionary> Dictionaries
        {
            get
            {
                return dictionaries;
            }
            set
            {
                dictionaries = value;
                OnPropertyChanged("Dictionaries");
            }
        }



        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
