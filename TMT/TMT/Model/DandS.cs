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
        private Dictionary _dict;
        private List<SuffixClass> _suffixes;
        private String _translationWord;
        private Boolean _isShown;
        private String _rawWord;
        private Boolean _skipTranslation;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="arg0">Dictionary</param>
        /// <param name="arg1">Suffixes</param>
        public DandS(Dictionary arg0, List<SuffixClass> arg1)
        {
            Dict = arg0;
            Suffixes = arg1;
            ShowDetails = new ShowDetailsCommand(this);
        }

        #region Getters and Setters
        /// <summary>
        /// Gets and sets the dict
        /// </summary>
        public Dictionary Dict
        {
            get
            {
                return _dict;
            }
            set
            {
                _dict = value;
                OnPropertyChanged("Dict");
            }
        }

        /// <summary>
        /// Gets and sets the suffix
        /// </summary>
        public List<SuffixClass> Suffixes
        {
            get
            {
                return _suffixes;
            }
            set
            {
                _suffixes = value;
                OnPropertyChanged("Suffixes");
            }
        }

        /// <summary>
        /// Gets or sets the translationWord
        /// </summary>
        public String TranslationWord
        {
            get
            {
                return _translationWord;
            }
            set
            {
                _translationWord = value;
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
                return _isShown;
            }
            set
            {
                _isShown = value;
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
                return _skipTranslation;
            }
            set
            {
                _skipTranslation = value;
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
                return _rawWord;
            }
            set
            {
                _rawWord = value;
                OnPropertyChanged("RawWord");
            }
        }
        #endregion
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
