using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.Model
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;
    
    [BsonIgnoreExtraElements]
    class SuffixClass : INotifyPropertyChanged
    {
        private String _tLSuffix;
        private String _sLSuffix;
        public ObjectId Id { get; set; }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="arg0">Source Language Suffix</param>
        /// <param name="arg1">Target Language Suffix</param>
        public SuffixClass(String arg0, String arg1)
        {
            SLSuffix = arg0;
            TLSuffix = arg1; 
        }

        #region Getters and Setters
        public string TLSuffix
        {
            get
            {
                return _tLSuffix;
            }
            set
            {
                _tLSuffix = value;
                OnPropertyChanged("TLSuffix");
            }
        }

        public string SLSuffix
        {
            get
            {
                return _sLSuffix;
            }
            set
            {
                _sLSuffix = value;
                OnPropertyChanged("SLSuffix");
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
