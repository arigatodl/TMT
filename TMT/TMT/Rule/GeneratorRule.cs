namespace TMT.Rule
{
    using System;
    using TMT.Mongolian;
    using MongoDB.Bson;

    public class GeneratorRule
    {
        public ObjectId Id { get; set; }
        private int _priority;
        private string _name;
        private MongolianWord _root;
        private MongolianWord _rootChangePart;
        private MongolianWord _rootChangeRule;
        private MongolianWord _suffix;
        private MongolianWord _suffixChangePart;
        private MongolianWord _suffixChangeRule;
        private MongolianWord _middle;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GeneratorRule() { }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public GeneratorRule(GeneratorRule rule) 
        {
            this.Id = rule.Id;
            this.Priority = rule.Priority;
            this.Name = rule.Name;
            this.Root = new MongolianWord(rule.Root);
            this.RootChangePart = new MongolianWord(rule.RootChangePart);
            this.RootChangeRule = new MongolianWord(rule.RootChangeRule);
            this.Suffix = new MongolianWord(rule.Suffix);
            this.SuffixChangePart = new MongolianWord(rule.SuffixChangePart);
            this.SuffixChangeRule = new MongolianWord(rule.SuffixChangeRule);
            this.Middle = new MongolianWord(rule.Middle);
        }

        /// <summary>
        /// Gets and Sets the _priority
        /// </summary>
        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }

        /// <summary>
        /// Gets and Sets the _name
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets and Sets the _root
        /// </summary>
        public MongolianWord Root
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
            }
        }

        /// <summary>
        /// Gets and Sets the _rootChangePart
        /// </summary>
        public MongolianWord RootChangePart
        {
            get
            {
                return _rootChangePart;
            }
            set
            {
                _rootChangePart = value;
            }
        }

        /// <summary>
        /// Gets and Sets the _rootChangeRule
        /// </summary>
        public MongolianWord RootChangeRule
        {
            get
            {
                return _rootChangeRule;
            }
            set
            {
                _rootChangeRule = value;
            }
        }

        /// <summary>
        /// Gets and Sets the _suffix
        /// </summary>
        public MongolianWord Suffix
        {
            get
            {
                return _suffix;
            }
            set
            {
                _suffix = value;
            }
        }

        /// <summary>
        /// Gets and Sets the _suffixChangePart
        /// </summary>
        public MongolianWord SuffixChangePart
        {
            get
            {
                return _suffixChangePart;
            }
            set
            {
                _suffixChangePart = value;
            }
        }

        /// <summary>
        /// Gets and Sets the _suffixChangeRule
        /// </summary>
        public MongolianWord SuffixChangeRule
        {
            get
            {
                return _suffixChangeRule;
            }
            set
            {
                _suffixChangeRule = value;
            }
        }

        /// <summary>
        /// Gets and Sets the _middle
        /// </summary>
        public MongolianWord Middle
        {
            get
            {
                return _middle;
            }
            set
            {
                _middle = value;
            }
        }
    }
}
