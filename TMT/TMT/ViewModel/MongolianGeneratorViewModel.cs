namespace TMT.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using TMT.Model;
    using TMT.Command;
    using System.Text;

    class MongolianGeneratorViewModel
    {
        private MongolianGeneratorModel _data;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MongolianGeneratorViewModel()
        {
            _data = new MongolianGeneratorModel();

            Execute = new GenerateCommand(this);
        }

        /// <summary>
        /// Gets the _data
        /// </summary>
        public MongolianGeneratorModel Data
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
        /// Generates the result by the rule
        /// </summary>
        public void Generate()
        {
            String root = Data.RawData.Split(' ')[0];
            List<string> suffixes = new List<string>();

            for (int i = 1; i < Data.RawData.Split(' ').Length; i++)
            {
                suffixes.Add(Data.RawData.Split(' ')[i]);
            }

            TMT.Rule.MongolianGenerator.Instance.Generate(root, suffixes);  // Generating

            Data.Results = TMT.Rule.MongolianGenerator.Instance.Results;
        }

        #region Commands
        /// <summary>
        /// Gets the TranslateViaText command for the ViewModel
        /// </summary>
        public ICommand Execute
        {
            get;
            private set;
        }
        #endregion
    }
}
