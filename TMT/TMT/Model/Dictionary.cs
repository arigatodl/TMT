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
    public class Dictionary :  INotifyPropertyChanged
    {
        private string tLWord;
        public ObjectId Id { get; set; }
        private string sLWord;
        private string type;
        private string suffix;
        private string tLSuffix;

        /// <summary>
        /// Parameterized Constructer
        /// </summary>
        /// <param name="arg0">Source Language Word</param>
        /// <param name="arg1">Target Language Word</param>
        /// <param name="arg2">Type</param>
        /// <param name="arg3">Suffix</param>
        public Dictionary(String arg0, String arg1, String arg2, String arg3)
        {
            SLWord = arg0;
            TLWord = arg1;
            Type = arg2;
            Suffix = arg3;
            TLSuffix = "";
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
        /// Gets TLSuffix of the word
        /// </summary>
        public string TLSuffix
        {
            get
            {
                return tLSuffix;
            }
            set
            {
                if (tLSuffix == null)
                {
                    tLSuffix = value;
                }
                else
                {
                    tLSuffix = value;
                    OnPropertyChanged("TLSuffix");
                }
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
                if (tLWord == null || tLWord == "")
                {
                    tLWord = value;
                }
                else
                {
                    tLWord = value;
                    OnPropertyChanged("TLWord");
                }
                
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
