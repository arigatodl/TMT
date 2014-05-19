using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TMT.Model
{
    using TMT.Command;
    using System.Windows.Input;

    class DandS : INotifyPropertyChanged
    {
        private Dictionary dict;
        private SuffixClass suffix;
        private String translationWord;
        private Boolean isShown;
        private String rawWord;
        private Boolean skipTranslation;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="arg0">Dictionary</param>
        /// <param name="arg1">SuffixClass</param>
        public DandS(Dictionary arg0, SuffixClass arg1)
        {
            dict = arg0;
            suffix = arg1;
            ShowDetails = new ShowDetailsCommand(this);
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

        /// <summary>
        /// Gets or sets the translationWord
        /// </summary>
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

        /// <summary>
        /// Gets or sets the isShown
        /// </summary>
        public Boolean IsShown
        {
            get
            {
                return isShown;
            }
            set
            {
                isShown = value;
                OnPropertyChanged("IsShown");
            }
        }

        /// <summary>
        /// Gets or sets the skipTranslation
        /// </summary>
        public Boolean SkipTranslation
        {
            get
            {
                return skipTranslation;
            }
            set
            {
                skipTranslation = value;
                OnPropertyChanged("SkipTranslation");
            }
        }

        /// <summary>
        /// Gets ShowDetails command for the ViewModel
        /// </summary>
        public ICommand ShowDetails
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the rawWord
        /// </summary>
        public String RawWord
        {
            get
            {
                return rawWord;
            }
            set
            {
                rawWord = value;
                OnPropertyChanged("RawWord");
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
