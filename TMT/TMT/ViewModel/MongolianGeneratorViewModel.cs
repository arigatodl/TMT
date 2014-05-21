namespace TMT.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using TMT.Model;

    class MongolianGeneratorViewModel
    {
        private ObservableCollection<MongolianGenerator> _generatedValues;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MongolianGeneratorViewModel()
        {
            _generatedValues = new ObservableCollection<MongolianGenerator>();
        }

        /// <summary>
        /// Gets the _generatedValues
        /// </summary>
        public ObservableCollection<MongolianGenerator> GeneratedValues
        {
            get
            {
                return _generatedValues;
            }
        }

        /// <summary>
        /// Generates the result by the rule
        /// </summary>
        public void Generate()
        {
        }
    }
}
