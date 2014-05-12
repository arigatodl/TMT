﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TMT.Model
{
    class Text: INotifyPropertyChanged
    {
        private List<Dictionary> words;
        private string rawData;
        private string extractedData;

        public List<Dictionary> Words
        {
            get
            {
                return words;
            }
            set
            {
                words = value;
                OnPropertyChanged("Words");
            }
        }

        public String RawData
        {
            get
            {
                return rawData;
            }
            set
            {
                rawData = value;
                OnPropertyChanged("RawData");
            }
        }

        public String ExtractedData
        {
            get
            {
                return extractedData;
            }
            set
            {
                extractedData = value;
                OnPropertyChanged("ExtractedData");
            }
        }

        public void ExtractWords(List<String> words, List<String> types)
        {
            extractedData = "";
            this.words = new List<Dictionary>();
            for (int i = 0; i < words.Count; i++)
            {
                Dictionary d = new Dictionary(words[i],null,types[i]);
                this.words.Add(d);
                extractedData += words[i] + ":" + types[i] + "\n";
            }
            OnPropertyChanged("ExtractedData");
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
