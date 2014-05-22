namespace TMT.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    class MongolianGeneratorModel : INotifyPropertyChanged
    {
        private List<string> _suffixes;
        private List<TMT.Rule.MongolianGeneratorResult> _results;
        private string _rawData;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MongolianGeneratorModel() { }

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
        /// Gets and Sets the _results
        /// </summary>
        public List<TMT.Rule.MongolianGeneratorResult> Results
        {
            get
            {
                return _results;
            }
            set
            {
                _results = value;
                OnPropertyChanged("Results");
            }
        }

        /// <summary>
        /// Gets and Sets the _rawData
        /// </summary>
        public string RawData
        {
            get
            {
                return _rawData;
            }
            set
            {
                _rawData = value;
                OnPropertyChanged("RawData");
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
