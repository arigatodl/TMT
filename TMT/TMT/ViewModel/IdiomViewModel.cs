namespace TMT.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using System.ComponentModel;
    using MongoDB.Driver;
    using TMT.Command;

    class IdiomViewModel: INotifyPropertyChanged
    {
        private string _sLText;
        private string _tLText;

        // Logical values
        string[] Words;

        public IdiomViewModel()
        {
            SLText = "";
            TLText = "";
            Save = new SaveCommand(this);
        }

        /// <summary>
        /// Creates the idiom tree and 
        /// saves to the DB
        /// </summary>
        public void SaveIdiom()
        {
            if (SLText.Length > 0)
            {
                MongoCursor<Idiom> _idiomCursor = Mongo.Instance.Database.GetCollection<Idiom>("Idioms").FindAll();
                Words = SLText.Split(' ');
                Boolean idiomExists = false;
                foreach (Idiom idiom in _idiomCursor)
                {
                    if (idiom.Value.Equals(Words[0]))
                    {
                        dfs(idiom, 1);
                        idiomExists = true;
                        debug(idiom);
                        Mongo.Instance.Database.GetCollection<Idiom>("Idioms").Save(idiom);
                        break;
                    }
                }
                if (idiomExists == false)
                {
                    Idiom temp = new Idiom();
                    TextToIdiom(temp, 0);
                    Mongo.Instance.Database.GetCollection<Idiom>("Idioms").Save(temp);
                }
            }
        }

        public void TextToIdiom(Idiom idiom, int a)
        {
            idiom.Value = Words[a];
            if (a + 1 >= Words.Length)
            {
                if (TLText.Length > 0)
                {
                    idiom.Translation = TLText;
                    idiom.IsEnd = true;
                }
                return;
            }
            idiom.Child = new List<Idiom>();
            Idiom temp = new Idiom();
            idiom.Child.Add(temp);
            TextToIdiom(temp, a + 1);
        }

        public void dfs(Idiom idiom, int a)
        {
            if (idiom.Child == null)
            {
                Idiom temp = new Idiom();
                idiom.Child.Add(temp);
                TextToIdiom(temp, a);
                return;
            }
            else
            {
                Boolean exists = false;
                for (int i = 0; i < idiom.Child.Count; i++)
                {
                    if (idiom.Child[i].Value.Equals(Words[a]))
                    {
                        exists = true;
                        if (a + 1 >= Words.Length)
                        {
                            idiom.Child[i].Translation = TLText;
                            idiom.Child[i].IsEnd = true;
                            return;
                        }
                        dfs(idiom.Child[i], a + 1);
                    }
                }
                if (exists == false)
                {
                    Idiom temp = new Idiom();
                    idiom.Child.Add(temp);
                    TextToIdiom(temp, a);
                }
            }
        }

        public void debug(Idiom idiom)
        {
            Console.Write(idiom.Value + " ");
            if(idiom.Child == null) return;
            Console.WriteLine(idiom.Child.Count);
            for (int i = 0; i < idiom.Child.Count; i++)
            {
                debug(idiom.Child[i]);
            }
        }

        #region Commands
        /// <summary>
        /// Gets the TranslateViaVoice command for the ViewModel
        /// </summary>
        public ICommand Save
        {
            get;
            private set;
        }
        #endregion
        #region Getters and Setters
        public string SLText
        {
            get
            {
                return _sLText;
            }
            set
            {
                _sLText = value;
                OnPropertyChanged("SLText");
            }
        }

        public string TLText
        {
            get
            {
                return _tLText;
            }
            set
            {
                _tLText = value;
                OnPropertyChanged("TLText");
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
