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

        private List<string> suffixes;

        /// <summary>
        /// Parameterized Constructer
        /// </summary>
        /// <param name="arg0">Source Language Word</param>
        /// <param name="arg1">Target Language Word</param>
        /// <param name="arg2">Type</param>
        /// <param name="arg3">Suffix</param>
        public Dictionary(String arg0, String arg1, String arg2, List<String> arg3)
        {
            SLWord = arg0;
            TLWord = arg1;
            Type = arg2;
            Suffixes = arg3;
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
                Console.WriteLine("Changed");
            }
        }

        
        /// <summary>
        /// Gets or sets the Type of the word 
        /// Ex. VB, Noun, ...
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

        /// <summary>
        /// Gets or sets the Suffix of the word
        /// Ex. Fut, Aor, Loc, ...
        /// </summary>
        public List<String> Suffixes
        {
            get
            {
                return suffixes;
            }
            set
            {
                suffixes = value;
                OnPropertyChanged("Suffixes");
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
