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

    class MainViewModel
    {
        private Text data;
        const string readPath = "C:\\Users\\Public\\iTranslator\\outputText.txt";
        const string writePath = "C:\\Users\\Public\\iTranslator\\inputText.txt";


        public MainViewModel()
        {
            data = new Text();
            TranslateViaText = new TranslateViaTextCommand(this);
        }

        /// <summary>
        /// Gets the data
        /// </summary>
        public Text Data
        {
            get
            {
                return data;
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

        public void writeText()
        {
            System.IO.File.WriteAllText(writePath, data.RawData);
        }

        public void loadWords()
        {
            string[] lines = System.IO.File.ReadAllLines(readPath);
            List<string> SLWords = new List<string>();
            List<string> Types = new List<string>();
            List<string> TLWords = new List<string>();
            
            MongoDatabase db = MongoDulguun.mongoServer.GetDatabase(MongoDulguun.databaseName);
            var dbCollection = db.GetCollection<Dictionary>("iWords");
            IMongoQuery query;

            foreach(string line in lines)
            {
                query = Query.And(
                    Query.Matches("SLWordDB", line));

                var filteredCollection = dbCollection.FindOne(query);
                if (filteredCollection != null)
                {
                    SLWords.Add(filteredCollection.SLWord);
                    Types.Add(filteredCollection.Type);
                    TLWords.Add(filteredCollection.TLWord);
                }
                else
                {
                    SLWords.Add("???");
                    Types.Add("UNK");
                    TLWords.Add("UNK");
                }
            }

            data.ExtractWords(SLWords, TLWords, Types);
            
            
            /*
            var s = t.FindAll();
            foreach (var haha in s)
            {
                SLWords.Add(haha.SLWord);
                Types.Add(haha.Type);
                TLWords.Add(haha.TLWord);
                Console.WriteLine(haha.Type);
            }
            data.ExtractWords(SLWords,TLWords,Types);*/

        }

        public void SeperateText()
        {
            writeText();
            iPackage.Convert.extract();
            loadWords();
        }
    }
}
