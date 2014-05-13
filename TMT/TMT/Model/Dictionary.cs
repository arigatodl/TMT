using System;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using System.Configuration;

namespace TMT.Model
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    [BsonIgnoreExtraElements]
    public class Dictionary : MongoEntity, INotifyPropertyChanged
    {
        [BsonElement("TLWordDB")]
        private string tLWord;
        
        [BsonElement("SLWordDB")]
        private string sLWord;

        [BsonElement("TypeDB")]
        private string type;

        /// <summary>
        /// Parameterized Constructer
        /// </summary>
        /// <param name="arg0">Source Language Word</param>
        /// <param name="arg1">Target Language Word List</param>
        /// <param name="arg2">Type</param>
        public Dictionary(String arg0, String arg1, String arg2)
        {
            SLWord = arg0;
            TLWord = arg1;
            Type = arg2;
        }
        
        /// <summary>
        /// Gets or sets the Source Language Word
        /// </summary>
        public String SLWord
        {
            get
            {
                return sLWord;
            }
            set
            {
                sLWord = value;
                OnPropertyChanged("SLWord");
            }
        }

        
        /// <summary>
        /// Gets or sets the Target Language Word
        /// </summary>
        public String TLWord
        {
            get
            {
                return tLWord;
            }
            set
            {
                tLWord = value;
                OnPropertyChanged("TLWord");
            }
        }

        
        /// <summary>
        /// Gets or sets the Type
        /// </summary>
        public String Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                OnPropertyChanged("Type");
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
