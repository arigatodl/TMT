namespace TMT.Rule
{
    using System;
    using System.ComponentModel;

    public class MongolianGeneratorResult: INotifyPropertyChanged
    {
        private TMT.Rule.GeneratorRule _rule;
        private string _result;
        private string _root;
        private string _suffix;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MongolianGeneratorResult() { }

        /// <summary>
        /// Gets and Sets the _rule
        /// </summary>
        public TMT.Rule.GeneratorRule Rule
        {
            get
            {
                return _rule;
            }
            set
            {
                _rule = value;
                OnPropertyChanged("Rule");
            }
        }

        /// <summary>
        /// Gets and Sets the _result
        /// </summary>
        public string Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                OnPropertyChanged("Result");
            }
        }

        /// <summary>
        /// Gets and Sets the _rule
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

        /// <summary>
        /// Gets and Sets the _suffix
        /// </summary>
        public string Suffix
        {
            get
            {
                return _suffix;
            }
            set
            {
                _suffix = value;
                OnPropertyChanged("Suffix");
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
