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
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Runtime.InteropServices;
    using System.Linq;
    using System.Threading;
    using Microsoft.Win32;
    using System.Speech.Recognition;
    using System.Globalization;
    using System.Speech.Synthesis;
    using System.IO;

    class MainViewModel:INotifyPropertyChanged
    {
        private string data;
        private Boolean changeState;
        private ObservableCollection<DandS> dandss;
        private Boolean isLoading;

        //Logical values
        List<SuffixClass> Suffixes;
        string Translation;
        int size;

        const string readPath = "C:\\Users\\Public\\iTranslator\\outputText.txt";
        const string writePath = "C:\\Users\\Public\\iTranslator\\inputText.txt";
        const string speechPath = "..\\..\\Resources\\Capture2Text\\Output\\speech_to_text.txt";
        const string generateWritePath = "..\\..\\Resources\\RFConvPre\\iGen.txt";
        const string generateReadPath = "..\\..\\Resources\\RFConvPre\\result_g.txt";

        public MainViewModel()
        {
            dandss = new AsyncObservableCollection<DandS>();
            IsLoading = false;

            TranslateViaText = new TranslateViaTextCommand(this);
            TranslateViaVoice = new TranslateViaVoiceCommand(this);
            AcceptUpdateDB = new AcceptUpdateDBCommand(this);
            CancelUpdateDB = new DeclineUpdateDBCommand(this);
            RuleWindow = new RuleWindowCommand(this);
        }

        /// <summary>
        /// Writes the input data to the output text file
        /// </summary>
        public void writeText()
        {
            System.IO.File.WriteAllText(writePath, data, Encoding.UTF8);
        }

        /// <summary>
        /// Extracts the output text file
        /// </summary>
        public void extract()
        {
                Suffixes = new List<SuffixClass>();
                dandss.Clear();
                changeState = false;

                string[] lines = System.IO.File.ReadAllLines(readPath,Encoding.UTF8);

                // Extracting morphologies from the text files
                int count = 0;
                foreach (string line in lines)
                {
                    count++;
                    if (count <= 1) continue;   // Skipping UTF8 file characters

                    string _sLWord;
                    string _type;
                    List<string> _suffixes = new List<string>();
                    
                    _sLWord = line.Split(';')[0];
                    _type = line.Split(';')[1];
                    if (line.Split(';').Length > 3) // If suffix exists
                    {
                        string temp;
                        foreach (string suffix in line.Split(';')[3].Split('+'))
                        {
                            temp = suffix;
                            if (suffix.EndsWith(")")) { temp = suffix.Remove(suffix.Length - 1); }
                            _suffixes.Add(temp);
                        }
                    }

                    Dictionary tempDictionary = null;
                    List<SuffixClass> tempSuffixClass = new List<SuffixClass>();

                    // Checking if the dand exists in the list before
                    foreach(DandS dand in dandss)
                    {
                        if (dand.Dict.SLWord.Equals(_sLWord) && dand.Dict.Type.Equals(_type))
                        {
                            Boolean dictExists = true; 
                            foreach (string newSuffix in _suffixes)
                            {
                                Boolean suffixExists = false;
                                foreach (string oldSuffix in dand.Dict.Suffixes)
                                {
                                    if (newSuffix.Equals(oldSuffix))
                                    {
                                        suffixExists = true;
                                        break;
                                    }
                                }
                                if (suffixExists == false) { dictExists = false; break; }
                            }
                            if (dictExists == true) { tempDictionary = dand.Dict; break; }
                        }
                    }

                    foreach(string newSuffix in _suffixes)
                    {
                        Boolean suffixExists = false;
                        foreach (SuffixClass suffixClass in Suffixes)
                        {
                            if (newSuffix.Equals(suffixClass.SLSuffix))
                            {
                                tempSuffixClass.Add(suffixClass);
                                suffixExists = true; break;
                            }
                        }
                        if (suffixExists == false)
                        {
                            SuffixClass temp = new SuffixClass(newSuffix, "");
                            Suffixes.Add(temp);
                            tempSuffixClass.Add(temp);
                        }
                    }


                    if (tempDictionary == null) tempDictionary = new Dictionary(_sLWord, "", _type, _suffixes);
                    dandss.Add(new DandS(tempDictionary, tempSuffixClass));
                }

                MongoCursor<Dictionary> _dictionaryCursor = Mongo.Instance.Database.GetCollection<Dictionary>("Words").FindAll();
                MongoCursor<SuffixClass> _suffixCursor = Mongo.Instance.Database.GetCollection<SuffixClass>("Suffixes").FindAll();
                
                //Suffixes
                foreach (SuffixClass d in Suffixes)
                {
                    Boolean suffixExists = false;
                    foreach (var suffix in _suffixCursor)
                    {
                        if (d.SLSuffix.Equals(suffix.SLSuffix))
                        {
                            d.Id = suffix.Id;
                            d.TLSuffix = suffix.TLSuffix;
                            suffixExists = true; break;
                        }
                    }
                    if (suffixExists == false)
                    {
                        d.TLSuffix = "";
                    }
                }
                
                // Searching from the database
                for(int i =0; i < dandss.Count; i++)
                {
                    DandS d = dandss[i];
                    Boolean dictExists = false;


                    // Dictionary
                    foreach(var dict in _dictionaryCursor)
                    {
                        if (dict.SLWord.Equals(d.Dict.SLWord) && dict.Type.Equals(d.Dict.Type))
                        {
                            if (dict.Suffixes.Count == d.Dict.Suffixes.Count)
                            {
                                foreach (string oldSuffix in d.Dict.Suffixes)
                                {
                                    Console.WriteLine(oldSuffix);
                                    Boolean suffixExists = false;
                                    foreach (string newSuffix in dict.Suffixes)
                                    {
                                        if (oldSuffix.Equals(newSuffix)) { suffixExists = true; dictExists = true; break; }
                                    }
                                    if (suffixExists == false)
                                    {
                                        dictExists = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (dictExists == true)
                        {
                            d.Dict.Id = dict.Id;
                            d.Dict.TLWord = dict.TLWord;
                            d.Dict.IsIllegal = dict.IsIllegal;
                            break;
                        }
                    }
                    if (dictExists == false)
                    {
                        Console.WriteLine(d.Dict.SLWord + "WTF");
                        foreach (var dict in _dictionaryCursor)
                        {
                            if (dict.SLWord.Equals(d.Dict.SLWord) && dict.Type.Equals(d.Dict.Type))
                            {
                                dictExists = true;
                                d.Dict.Id = dict.Id;
                                d.Dict.TLWord = dict.TLWord;
                                d.Dict.IsIllegal = dict.IsIllegal;
                                break;
                            }
                        }
                        if (dictExists == false)
                        {
                            foreach (var dict in _dictionaryCursor)
                            {
                                if (dict.SLWord.Equals(d.Dict.SLWord))
                                {
                                    dictExists = true;
                                    d.Dict.Id = dict.Id;
                                    d.Dict.TLWord = dict.TLWord;
                                    d.Dict.IsIllegal = dict.IsIllegal;
                                    break;
                                }
                            }
                            if (dictExists == false)
                            {
                                d.Dict.TLWord = d.Dict.SLWord;
                                d.Dict.IsIllegal = false;
                            }
                        }
                    }
                }
        }

        /// <summary>
        /// Checks 
        /// </summary>
        public void checkIdioms(int a)
        {
            MongoCursor<Idiom> _idiomCursor = Mongo.Instance.Database.GetCollection<Idiom>("Idioms").FindAll();

            foreach (Idiom idiom in _idiomCursor)
            {
                if (idiom.Value.Equals(dandss[a].Dict.SLWord))
                {
                    dfs(idiom, a + 1);
                }
            }
        }

        /// <summary>
        /// Depth First Search for Idiom
        /// </summary>
        public void dfs(Idiom idiom, int a)
        {
            if (idiom.Child == null) return;
            for (int i = 0; i < idiom.Child.Count; i++)
            {
                if (idiom.Child[i].Value.Equals(dandss[a].Dict.SLWord))
                {
                    if (idiom.Child[i].IsEnd == true)
                    {
                        Translation = idiom.Child[i].Translation;
                        size = a;
                    }
                    dfs(idiom.Child[i], a + 1);
                }
            }
        }

        /// <summary>
        /// Generates the words from the RFConv
        /// </summary>
        public void generate()
        {
            /*string data = "";
            foreach(DandS d in dandss)
            {
                data += d.Dict.TLWord + "+";
                if (d.Dict.TLSuffix != null && d.Dict.TLSuffix != "")
                {
                    data += d.Dict.TLSuffix;
                }
                else
                {
                    data += d.Suffix.TLSuffix;
                }
                data += Environment.NewLine;
            }

            System.IO.File.WriteAllText(generateWritePath, data, Encoding.UTF8);

            string fullPath = "..\\..\\Resources\\RFConvPre\\uKimmo.exe";
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Path.GetFileName(fullPath);
            psi.WorkingDirectory = Path.GetDirectoryName(fullPath);
            psi.Arguments = "mon.rul rullex/mon.lex iGen.txt iRec.txt";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            var process = Process.Start(psi);
            process.WaitForExit();

            string[] lines = System.IO.File.ReadAllLines(generateReadPath);

            int i = 0;
            foreach(string line in lines){
                if (line == "")
                {
                    if (dandss[i].SkipTranslation == false)
                    {
                        if (data.Equals("*** NONE ***")) dandss[i].TranslationWord = dandss[i].Dict.SLWord;
                        else dandss[i].TranslationWord = data;
                        if (dandss[i].Suffix.TLSuffix.Contains(' ') && (dandss[i].Dict.TLSuffix == null || dandss[i].Dict.TLSuffix == ""))
                        {
                            dandss[i].TranslationWord += (" " + dandss[i].Suffix.TLSuffix.Split(' ')[1]);
                        }
                        if (i + 1 < dandss.Count)
                        {
                            Console.WriteLine(dandss[i + 1].TranslationWord + ":" + dandss[i].TranslationWord);
                            if (dandss[i + 1].Dict.SLWord == "?") dandss[i].TranslationWord += " вэ";
                        }
                    }
                    i++;
                }
                else data = line;
            }*/

            foreach(DandS d in dandss)
            {
                if (d.Dict.IsIllegal == false)
                {
                    List<string> _suffixes = new List<string>();
                    foreach (var suffix in d.Suffixes)
                    {
                        if (suffix.TLSuffix != "")
                            _suffixes.Add(suffix.TLSuffix);
                    }
                    d.TranslationWord = TMT.Rule.MongolianGenerator.Instance.Generate(d.Dict.TLWord, _suffixes);
                }
                else
                {
                    d.TranslationWord = d.Dict.TLWord;
                }
            }
        }

        /// <summary>
        /// Main Translation procedure
        /// </summary>
        public void SeperateText()
        {
            IsLoading = true;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Sets IsLoading = false when worker finishes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsLoading = false;
        }

        /// <summary>
        /// Does heavy jobs on the background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            writeText();
            iPackage.Convert.extract();
            extract();
            generate();
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,
           UIntPtr dwExtraInfo);

        /// <summary>
        /// Runs the SpeechRecognition and saves the output to the data.RawData
        /// </summary>
        public void Listen()
        {
            /*uint KEYEVENTF_KEYUP = 0x0002;
            uint KEYEVENTF_EXTENDEDKEY = 0x0001;

            keybd_event((byte)0x5B, (byte)0, (uint)KEYEVENTF_EXTENDEDKEY | 0, (UIntPtr) 0);
            keybd_event((byte)0x41, (byte)0, (uint)KEYEVENTF_EXTENDEDKEY | 0, (UIntPtr)0);
            keybd_event((byte)0x5B, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            keybd_event((byte)0x41, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);

                SpeechSynthesizer synthesizer = new SpeechSynthesizer();
                synthesizer.Volume = 100;  // 0...100
                synthesizer.Rate = -2;     // -10...10
                synthesizer.SelectVoice("MN MALE TTSVoice");
                
                synthesizer.Speak("Сайн байна уу?");*/
        }

        /// <summary>
        /// Updates dictionaries field if exists else inserts field into the database
        /// </summary>
        public void Update()
        {
            foreach(DandS d in dandss){
                if (!d.Dict.SLWord.Equals(d.Dict.TLWord))
                {
                    Mongo.Instance.Database.GetCollection<Dictionary>("Words").Save(d.Dict);
                }
            }
            foreach (var suffix in Suffixes)
            {
                if (suffix.TLSuffix != "")
                {
                    Mongo.Instance.Database.GetCollection<Dictionary>("Suffixes").Save(suffix);
                }
            }

            extract();
            generate();
        }

        /// <summary>
        /// Refreshes the dictionaries
        /// </summary>
        public void Decline()
        {
            extract();
            generate();
        }

        /// <summary>
        /// Shows the Rule window
        /// </summary>
        public void ShowRuleWindow()
        {
            TMT.View.MongolianGeneratorView window = new View.MongolianGeneratorView();
            window.ShowDialog();
        }

        #region Getters and Setters
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

        /// <summary>
        /// Gets changeState
        /// </summary>
        public Boolean ChangeState
        {
            get
            {
                return changeState;
            }
            set
            {
                changeState = value;
                OnPropertyChanged("ChangeState");
            }
        }

        /// <summary>
        /// Gets dandsd\s
        /// </summary>
        public ObservableCollection<DandS> Dandss
        {
            get
            {
                return dandss;
            }
        }

        /// <summary>
        /// Gets or sets isLoading
        /// </summary>
        public Boolean IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }
        #endregion
        #region Commands
        /// <summary>
        /// Gets the TranslateViaText command for the ViewModel
        /// </summary>
        public ICommand TranslateViaText
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the TranslateViaVoice command for the ViewModel
        /// </summary>
        public ICommand TranslateViaVoice
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the AcceptUpdateDB command for the ViewModel
        /// </summary>
        public ICommand AcceptUpdateDB
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the CancelUpdateDB command for the ViewModel
        /// </summary>
        public ICommand CancelUpdateDB
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the RuleWindow command for the ViewModel
        /// </summary>
        public ICommand RuleWindow
        {
            get;
            private set;
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
