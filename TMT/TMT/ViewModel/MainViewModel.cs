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
            List<string> words = new List<string>();
            List<string> types = new List<string>();
            foreach (string line in lines)
            {
                words.Add(line);
                types.Add("LOL");
            }
            data.ExtractWords(words,types);
            Console.WriteLine("DONE");
        }

        public void SeperateText()
        {
            writeText();
            iPackage.Convert.extract();
            loadWords();
        }
    }
}
