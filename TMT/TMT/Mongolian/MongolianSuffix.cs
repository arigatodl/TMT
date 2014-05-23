namespace TMT.Mongolian
{
    using System;

    public class MongolianSuffix: MongolianWord
    {
        private int _priority;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MongolianSuffix() { }

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

    }
}
