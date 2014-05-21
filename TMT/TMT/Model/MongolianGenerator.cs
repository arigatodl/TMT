namespace TMT.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    class MongolianGenerator : INotifyPropertyChanged
    {
        private List<string> _suffixes;
        private string _root;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MongolianGenerator() { }

        /// <summary>
        /// Gets and Sets the _suffixes
        /// </summary>
        public List<string> Suffixes
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
        /// Gets and Sets the _root
        /// </summary>
        public string Root
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
                OnPropertyChanged("Root");
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
