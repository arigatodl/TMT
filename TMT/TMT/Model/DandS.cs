using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.Model
{
    class DandS : INotifyPropertyChanged
    {
        private Dictionary dict;
        private SuffixClass suffix;
        private String translationWord; 

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="arg0">Dictionary</param>
        /// <param name="arg1">SuffixClass</param>
        public DandS(Dictionary arg0, SuffixClass arg1)
        {
            dict = arg0;
            suffix = arg1;
        }

        /// <summary>
        /// Gets and sets the dict
        /// </summary>
        public Dictionary Dict
        {
            get
            {
                return dict;
            }
            set
            {
                dict = value;
                OnPropertyChanged("Dict");
            }
        }

        /// <summary>
        /// Gets and sets the suffix
        /// </summary>
        public SuffixClass Suffix
        {
            get
            {
                return suffix;
            }
            set
            {
                suffix = value;
                OnPropertyChanged("Suffix");
            }
        }

        public String TranslationWord
        {
            get
            {
                return translationWord;
            }
            set
            {
                translationWord = value;
                OnPropertyChanged("TranslationWord");
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
