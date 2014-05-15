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

    class MainViewModel:INotifyPropertyChanged
    {
        private string data;
        private Boolean changeState;
        private ObservableCollection<DandS> dandss;
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
            dandss = new ObservableCollection<DandS>();

            dandss.CollectionChanged += dandss_CollectionChanged;
            TranslateViaText = new TranslateViaTextCommand(this);
            TranslateViaVoice = new TranslateViaVoiceCommand(this);
            AcceptUpdateDB = new AcceptUpdateDBCommand(this);
            CancelUpdateDB = new DeclineUpdateDBCommand(this);
        }

        void dandss_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChangeState = true;
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
            try
            {
                MongoDatabase db = MongoDulguun.mongoServer.GetDatabase(MongoDulguun.databaseName);
                IMongoQuery query;

                dandss.Clear();
                changeState = false;

                string[] lines = System.IO.File.ReadAllLines(readPath);

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
                            if(!temp.Equals("Nom") && !temp.Equals("P3sg")) Suffix = temp;
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
                }

                var dbCollection1 = db.GetCollection<Dictionary>("skipTypes");
                var dbCollection2 = db.GetCollection<Dictionary>("iWords");
                var dbCollection3 = db.GetCollection<SuffixClass>("iSuffixes");

                foreach (DandS d in dandss)
                {
                    query = Query.And(
                        Query.Matches("Type", d.Dict.Type)
                        );
                    var filteredCollection1 = dbCollection1.FindOne(query);
                    if (filteredCollection1 != null)
                    {
                        d.Dict.TLWord = d.Dict.SLWord;
                    }
                    else
                    {
                        query = Query.And(
                                    Query.Matches("SLWord", d.Dict.SLWord),
                                    Query.Matches("Type", d.Dict.Type),
                                    Query.Matches("Suffix", d.Dict.Suffix)
                                );
                        var filteredCollection2 = dbCollection2.FindOne(query);

                        if (filteredCollection2 != null)
                        {
                            d.Dict.Id = filteredCollection2.Id;
                            d.Dict.TLWord = (filteredCollection2.TLWord);

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
                        else
                        {
                            d.Dict.TLWord = "???";
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
            }
            catch
            {
                Console.WriteLine("Database Error");
            }
        }

        public void generate()
        {
            string data = "";
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

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Users\Dulguun\Documents\GitHub\TMT\TMT\TMT\Resources\RFConvPre\uKimmo.exe";
            startInfo.Arguments = @"C:\Users\Dulguun\Documents\GitHub\TMT\TMT\TMT\Resources\RFConvPre\mon.rul C:\Users\Dulguun\Documents\GitHub\TMT\TMT\TMT\Resources\RFConvPre\rulles\mon.lex C:\Users\Dulguun\Documents\GitHub\TMT\TMT\TMT\Resources\RFConvPre\iGen.txt C:\Users\Dulguun\Documents\GitHub\TMT\TMT\TMT\Resources\RFConvPre\iRec.txt";
            Process.Start(startInfo);
           // }
           // catch
           // {
           //  throw new Exception("Error");
           // }
        }

        /// <summary>
        /// Main Translation procedure
        /// </summary>
        public void SeperateText()
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
            uint KEYEVENTF_KEYUP = 0x0002;
            uint KEYEVENTF_EXTENDEDKEY = 0x0001;

            keybd_event((byte)0x5B, (byte)0, (uint)KEYEVENTF_EXTENDEDKEY | 0, (UIntPtr) 0);
            keybd_event((byte)0x41, (byte)0, (uint)KEYEVENTF_EXTENDEDKEY | 0, (UIntPtr)0);
            keybd_event((byte)0x5B, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            keybd_event((byte)0x41, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);

                //var sp = new SpeechRecognitionEngine(new CultureInfo("mn-MN"));
                SpeechSynthesizer synthesizer = new SpeechSynthesizer();
                synthesizer.Volume = 100;  // 0...100
                synthesizer.Rate = -2;     // -10...10
                synthesizer.SelectVoice("MN MALE TTSVoice");
                foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
                {
                    VoiceInfo info = voice.VoiceInfo;

                    Console.WriteLine(" Name:          " + info.Name);
                    Console.WriteLine(" Culture:       " + info.Culture);
                    Console.WriteLine(" Age:           " + info.Age);
                    Console.WriteLine(" Gender:        " + info.Gender);
                    Console.WriteLine(" Description:   " + info.Description);
                    Console.WriteLine(" ID:            " + info.Id);
                }
                // Synchronous
                //synthesizer.Speak("Сайн байна уу?");
        }

        /// <summary>
        /// Updates dictionaries field if exists else inserts field into the database
        /// </summary>
        public void Update()
        {
            MongoDatabase db = MongoDulguun.mongoServer.GetDatabase(MongoDulguun.databaseName);
            var dbCollection1 = db.GetCollection<Dictionary>("iWords");
            var dbCollection2 = db.GetCollection<Dictionary>("iSuffixes");
            foreach(DandS d in dandss){
                try
                {
                    dbCollection1.Save(d.Dict);
                }
                catch { }
                try
                {
                    dbCollection2.Save(d.Suffix);
                }
                catch { }
            }

            ChangeState = false;
            extract();
            generate();
        }

        /// <summary>
        /// Refreshes the dictionaries
        /// </summary>
        public void Decline()
        {
            extract();
            ChangeState = false;
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
