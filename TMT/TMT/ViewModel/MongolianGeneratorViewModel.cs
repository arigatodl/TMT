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
            Save = new SaveRuleCommand(this);
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
                Console.WriteLine(suffixes[i-1]);
            }

            TMT.Rule.MongolianGenerator.Instance.Generate(root, suffixes);  // Generating

            Data.Results = TMT.Rule.MongolianGenerator.Instance.Results;

            for (int i = 0; i < TMT.Rule.MongolianGenerator.Instance.Results.Count; i++)
            {
                Console.WriteLine(TMT.Rule.MongolianGenerator.Instance.ResultWord.Word);
            }
        }

        /// <summary>
        /// Saves the rule to the mongodb
        /// </summary>
        public void SaveRule()
        {
            if (TMT.Rule.MongolianGenerator.Instance.Results != null)
            {
                Console.WriteLine("YES");
                foreach (var rule in TMT.Rule.MongolianGenerator.Instance.Results)
                {
                    if (rule.Rule.Root.Word != null && rule.Rule.Root.Word != "")
                    {
                        Mongo.Instance.Database.GetCollection<TMT.Rule.GeneratorRule>("GeneratorRuleTable").Save(rule.Rule);
                    }
                }
            }
            else Console.WriteLine("NO");
        }

        #region Commands
        /// <summary>
        /// Gets the Execute command for the ViewModel
        /// </summary>
        public ICommand Execute
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Save command for the ViewModel
        /// </summary>
        public ICommand Save
        {
            get;
            private set;
        }
        #endregion
    }
}
