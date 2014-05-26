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

        // Checkers
        private Boolean isException;

        const string readPath = "C:\\Users\\Public\\iTranslator\\outputText.txt";
        const string writePath = "C:\\Users\\Public\\iTranslator\\inputText.txt";
        const string speechPath = "..\\..\\Resources\\Capture2Text\\Output\\speech_to_text.txt";
        const string generateWritePath = "..\\..\\Resources\\RFConvPre\\iGen.txt";
        const string generateReadPath = "..\\..\\Resources\\RFConvPre\\result_g.txt";

        string SLWord;
        string Type;
        string Suffix;

        public MainViewModel()
        {
            dandss = new AsyncObservableCollection<DandS>();
            IsLoading = false;

            dandss.CollectionChanged += dandss_CollectionChanged;
            TranslateViaText = new TranslateViaTextCommand(this);
            TranslateViaVoice = new TranslateViaVoiceCommand(this);
            AcceptUpdateDB = new AcceptUpdateDBCommand(this);
            CancelUpdateDB = new DeclineUpdateDBCommand(this);
        }

        /// <summary>
        /// Adds event handlers to Dands objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dandss_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DandS item in e.OldItems)
                {
                    item.Dict.PropertyChanged -= EntityViewModelPropertyChanged;
                    item.Suffix.PropertyChanged -= EntityViewModelPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DandS item in e.NewItems)
                {
                    item.Dict.PropertyChanged += EntityViewModelPropertyChanged;
                    item.Suffix.PropertyChanged += EntityViewModelPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Notifies if TLSuffix or TLWord is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntityViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("TLSuffix") || e.PropertyName.Equals("TLWord"))
            {
                if (isException) return; 
                ChangeState = true;
            }
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

        #endregion

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
                IMongoQuery query;

                dandss.Clear();
                changeState = false;
                isException = true;

                string[] lines = System.IO.File.ReadAllLines(readPath,Encoding.UTF8);

                int count = 0;
                foreach (string line in lines)
                {
                    count++;
                    if (count <= 1) continue;   // Skipping UTF8 file characters
                    SLWord = line.Split(';')[0];
                    Type = line.Split(';')[1];
                    Suffix = "";

                    if (line.Split(';').Length > 3)
                    {
                        string temp;
                        foreach (string suffix in line.Split(';')[3].Split('+'))
                        {
                            temp = suffix;
                            if (suffix.EndsWith(")")) { temp = suffix.Remove(suffix.Length - 1); }
                            if (!temp.Equals("Nom") && !temp.Equals("Pnon") && !temp.Equals("A2sg") && !temp.Equals("A3sg")) Suffix = temp;
                        }
                    }


                    Dictionary tempDictionary = null;
                    SuffixClass tempSuffixClass = null;

                    foreach(DandS dand in dandss)
                    {
                        if (dand.Dict.SLWord.Equals(SLWord) && dand.Dict.Type.Equals(Type) && dand.Dict.Suffix.Equals(Suffix))
                        {
                            tempDictionary = dand.Dict;
                            break;
                        }
                    }

                    foreach (DandS dand in dandss)
                    {
                        if (dand.Suffix.Suffix.Equals(Suffix))
                        {
                            tempSuffixClass = dand.Suffix;
                            break;
                        }
                    }

                    if (tempSuffixClass == null) tempSuffixClass = new SuffixClass(Suffix, "");
                    if (tempDictionary == null) tempDictionary = new Dictionary(SLWord, "", Type, Suffix);
                    dandss.Add(new DandS(tempDictionary, tempSuffixClass));
                    Console.WriteLine(SLWord + ":" + count);
                }

                var dbCollection1 = Mongo.Instance.Database.GetCollection<Dictionary>("skipTypes");
                var dbCollection2 = Mongo.Instance.Database.GetCollection<Dictionary>("iWords");
                var dbCollection3 = Mongo.Instance.Database.GetCollection<SuffixClass>("iSuffixes");

                foreach (DandS d in dandss)
                {
                    query = Query.And(
                        Query.Matches("Type", d.Dict.Type)
                        );
                    var filteredCollection1 = dbCollection1.FindOne(query);
                    if (filteredCollection1 != null)
                    {
                        d.Dict.TLWord = d.Dict.SLWord;
                        d.TranslationWord = d.Dict.SLWord;
                        d.SkipTranslation = true;
                    }
                    else
                    {
                        d.SkipTranslation = false;
                        query = Query.And(
                                    Query.Matches("SLWord", d.Dict.SLWord),
                                    Query.Matches("Type", d.Dict.Type),
                                    Query.Matches("Suffix", d.Dict.Suffix)
                                );
                        var filteredCollection2 = dbCollection2.FindOne(query);

                        if (filteredCollection2 != null)
                        {
                            d.Dict.Id = filteredCollection2.Id;
                            d.Dict.TLWord = filteredCollection2.TLWord;
                            if (d.Dict.TLWord.Contains(' '))
                            {
                                d.TranslationWord = d.Dict.TLWord;
                                d.SkipTranslation = true;
                            }

                            query = Query.And(
                                       Query.Matches("SLWord", d.Dict.SLWord),
                                       Query.Matches("Type", d.Dict.Type),
                                       Query.Matches("Suffix", d.Dict.Suffix),
                                       Query.Matches("TLSuffix", d.Dict.TLSuffix)
                                    );
                            var filteredCollection3 = dbCollection2.FindOne(query);

                            if (filteredCollection3 != null)
                            {
                                d.Dict.TLSuffix = filteredCollection3.TLSuffix;
                            }
                        }
                        else{
                            query = Query.And(
                                    Query.Matches("SLWord", d.Dict.SLWord)
                                );
                            var filteredCollection5 = dbCollection2.FindOne(query);
                            if(filteredCollection5 != null){
                                d.Dict.Id = filteredCollection5.Id;
                                d.Dict.TLWord = filteredCollection5.TLWord;
                            }
                            else
                            {
                                d.Dict.TLWord = "???";
                            }
                        }
                        
                        query = Query.And(
                                       Query.Matches("Suffix", d.Dict.Suffix)
                                    );
                        var filteredCollection4 = dbCollection3.FindOne(query);
                        if (filteredCollection4 != null)
                        {
                            d.Suffix.Id = filteredCollection4.Id;
                            d.Suffix.TLSuffix = filteredCollection4.TLSuffix;
                        }
                    }
                }
                isException = false;
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

            List<string> data = new List<string>();

            foreach(DandS d in dandss)
            {
                if (d.Dict.TLSuffix != null && d.Dict.TLSuffix != "")
                {
                    data.Add(d.Dict.TLSuffix);
                }
                else
                {
                    data.Add(d.Suffix.TLSuffix);
                }

                d.TranslationWord = TMT.Rule.MongolianGenerator.Instance.Generate(d.Dict.TLWord, data);
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
            isException = false;
            var dbCollection1 = Mongo.Instance.Database.GetCollection<Dictionary>("iWords");
            var dbCollection2 = Mongo.Instance.Database.GetCollection<Dictionary>("iSuffixes");
            foreach(DandS d in dandss){
                dbCollection1.Save(d.Dict);
                dbCollection2.Save(d.Suffix);
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
