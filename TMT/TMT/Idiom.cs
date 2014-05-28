namespace TMT
{
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using MongoDB.Bson;

    class Idiom: INotifyPropertyChanged
    {
        public ObjectId Id { get; set; }
        private string _value;
        private Boolean _isEnd;
        private string _translation;
        private List<Idiom> _child;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Idiom()
        {
        }

        #region Getters and Setters
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public Boolean IsEnd
        {
            get
            {
                return _isEnd;
            }
            set
            {
                _isEnd = value;
                OnPropertyChanged("IsEnd");
            }
        }

        public string Translation
        {
            get
            {
                return _translation;
            }
            set
            {
                _translation = value;
                OnPropertyChanged("Translation");
            }
        }

        public List<Idiom> Child
        {
            get
            {
                return _child;
            }
            set
            {
                _child = value;
                OnPropertyChanged("Child");
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
