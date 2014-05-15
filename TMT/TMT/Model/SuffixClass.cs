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
        private String tLSuffix;
        private String suffix;
        public ObjectId Id { get; set; }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="arg0">Suffix</param>
        /// <param name="arg1">TLSuffix</param>
        public SuffixClass(String arg0, String arg1)
        {
            Suffix = arg0;
            TLSuffix = arg1; 
        }

        public string TLSuffix
        {
            get
            {
                return tLSuffix;
            }
            set
            {
                tLSuffix = value;
                OnPropertyChanged("TLSuffix");
            }
        }

        public string Suffix
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
