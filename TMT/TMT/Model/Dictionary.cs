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
        private string _tLWord;
        public ObjectId Id { get; set; }
        private string _sLWord;
        private string _type;
        private List<string> _suffixes;
        private Boolean _isIllegal;

        /// <summary>
        /// Parameterized Constructer
        /// </summary>
        /// <param name="arg0">Source Language Word</param>
        /// <param name="arg1">Target Language Word</param>
        /// <param name="arg2">Type</param>
        /// <param name="arg3">Suffixes</param>
        public Dictionary(String arg0, String arg1, String arg2, List<String> arg3)
        {
            SLWord = arg0;
            TLWord = arg1;
            Type = arg2;
            Suffixes = arg3;
        }
        
        #region Getters and Setters
        /// <summary>
        /// Gets or sets the Source Language Word
        /// </summary>
        public String SLWord
        {
            get
            {
                return _sLWord;
            }
            set
            {
                _sLWord = value;
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
                return _tLWord;
            }
            set
            {
                _tLWord = value;
                OnPropertyChanged("TLWord");
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
                return _type;
            }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        /// <summary>
        /// Gets or sets the Suffix of the word
        /// Ex. Fut, Aor, Loc, ...
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
        /// Gets or sets the isIllegal of the word
        /// Ex. би ын = миний
        /// </summary>
        public Boolean IsIllegal
        {
            get
            {
                return _isIllegal;
            }
            set
            {
                _isIllegal = value;
                OnPropertyChanged("IsIllegal");
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
