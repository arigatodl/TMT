using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TMT.Model;
using TMT.Command;
using zemberek.morphology.apps;
using zemberek.morphology.ambiguity;
using zemberek.morphology.parser;
using System.Collections.Generic;

namespace TMT.ViewModel
{
    class MainViewModel
    {
        private Text data;

        public MainViewModel()
        {
            data = new Text();
            data.RawData = "aaaa";
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

        public void SeperateText()
        {
            TurkishMorphParser parser = TurkishMorphParser.createWithDefaults();
		    TurkishSentenceParser sentenceParser = new TurkishSentenceParser(parser, new Z3MarkovModelDisambiguator());

            List<MorphParse> parseRes = (List<MorphParse>)sentenceParser.bestParse(Data.RawData);
	        foreach (MorphParse p in parseRes) 
            {
	                Console.WriteLine(p.getLemma());
	        }
            
        }
    }
}
