using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TMT.Model;
using TMT.Command;

namespace TMT.ViewModel
{
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoDB.Bson;
    using System.Collections.ObjectModel;

    class MainViewModel
    {
        private string data;
        private Boolean changeState;
        private ObservableCollection<Dictionary> dictionaries;
        const string readPath = "C:\\Users\\Public\\iTranslator\\outputText.txt";
        const string writePath = "C:\\Users\\Public\\iTranslator\\inputText.txt";

        string SLWord;
        string Type;
        string TLWord;
        List<string> Suffixes;

        public MainViewModel()
        {
            dictionaries = new ObservableCollection<Dictionary>();
            TranslateViaText = new TranslateViaTextCommand(this);
            TranslateViaVoice = new TranslateViaVoiceCommand(this);
        }

        /// <summary>
        /// Gets data
        /// </summary>
        public string Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public Boolean ChangeState
        {
            get
            {
                return changeState;
            }
        }

        /// <summary>
        /// Gets dictionaies
        /// </summary>
        public ObservableCollection<Dictionary> Dictionaries
        {
            get
            {
                return dictionaries;
            }
        }


        /// <summary>
        /// Gets the TranslateViaText command for the ViewModel
        /// </summary>
        public ICommand TranslateViaText
        {
            get;
            private set;
        }

        public ICommand TranslateViaVoice
        {
            get;
            private set;
        }

        /// <summary>
        /// Writes the input data to the output text file
        /// </summary>
        public void writeText()
        {
            System.IO.File.WriteAllText(writePath, data);
        }

        /// <summary>
        /// Extracts the output text file
        /// </summary>
        public void extract()
        {
            try
            {
                MongoDatabase db = MongoDulguun.mongoServer.GetDatabase(MongoDulguun.databaseName);
                IMongoQuery query;

                dictionaries.Clear();
                changeState = false;

                string[] lines = System.IO.File.ReadAllLines(readPath);
                Suffixes = new List<string>();

                foreach (string line in lines)
                {
                    SLWord = line.Split(';')[0];
                    Type = line.Split(';')[1];

                    if (line.Split(';').Length > 3)
                    {
                        string temp;
                        foreach (string suffix in line.Split(';')[3].Split('+'))
                        {
                            temp = suffix;
                            if (suffix.EndsWith(")")) { temp = suffix.Remove(suffix.Length - 1); }
                            Suffixes.Add(temp);
                            Console.WriteLine(temp);
                        }
                    }

                    dictionaries.Add(new Dictionary(SLWord, "", Type, Suffixes));
                }

                var dbCollection1 = db.GetCollection<Dictionary>("skipTypes");
                var dbCollection2 = db.GetCollection<Dictionary>("iWords");

                foreach (Dictionary d in dictionaries)
                {
                    query = Query.And(
                        Query.Matches("TypeDB", d.Type)
                        );
                    var filteredCollection1 = dbCollection1.FindOne(query);
                    if (filteredCollection1 != null)
                    {
                        d.TLWord = d.SLWord;
                    }

                    query = Query.And(
                            Query.Matches("SLWordDB", d.SLWord)
                            );
                    var filteredCollection2 = dbCollection2.FindOne(query);
                    if (filteredCollection2 != null)
                    {
                        d.TLWord = (filteredCollection2.TLWord);
                    }
                    else
                    {
                        d.TLWord = "UNK";
                    }
                }
            }
            catch
            {
                Console.WriteLine("Database Error");
            }
        }


        /// <summary>
        /// Main Translation procedure
        /// </summary>
        public void SeperateText()
        {
            writeText();
            iPackage.Convert.extract();
            Console.WriteLine("Converted");
            extract();
        }

        /// <summary>
        /// Runs the SpeechRecognition and saves the output to the data.RawData
        /// </summary>
        public void Listen()
        {

        }
    }
}
