namespace TMT.Mongolian
{
    using System;

    public static class Letter
    {
        public enum letterType   // Үсгийн төрөл.
        {
            Vowel,
            Consonant,
            Sign,
            Unknown
        };

        /**
         *    а о у я ё э ө ү е ю и й
         * эр 0 0 у я ё 0 0 у е ю и й 
         * эм 0 0 ү я ё 0 0 ү е ю и й
         * */
        private static char[,] _ruleArray = 
        {
            {'0', '0', 'у', 'я', 'ё', '0', '0', 'у', 'е', 'ю', 'и', 'й'},
            {'0', '0', 'ү', 'я', 'ё', '0', '0', 'ү', 'е', 'ю', 'и', 'й'}
        };

        private static char[] _vowels = { 'а', 'о', 'у', 'я', 'ё', 'э', 'ө', 'ү', 'е', 'ю', 'и', 'й'};   //Эгшиг
        private static char[] _consonants = { 'б', 'в', 'г', 'д', 'ж', 'з', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ' };  //Гийгүүлэгч
        private static char[] _signs = { 'ъ', 'ь' }; // Тэмдэг
        

        /// <summary>
        /// Checks if the given char is Mongolian vowel
        /// 
        /// Тухайн char нь Монгол хэлний эгшиг эсэхийг буцаана
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static letterType GetLetterType(this Char C)
        {
            /// Checking if it is a vowel
            for (int i = 0; i < _vowels.Length; i++)
            {
                if (_vowels[i] == C) return letterType.Vowel;
            }

            /// Checking if it is an consonant
            for (int i = 0; i < _consonants.Length; i++)
            {
                if (_consonants[i] == C) return letterType.Consonant;
            }

            /// Checking if it is a sign
            for (int i = 0; i < _signs.Length; i++)
            {
                if (_signs[i] == C) return letterType.Sign;
            }

            return letterType.Unknown;
        }

        /// <summary>
        /// Returns the gender of the char. If the char is not Mongolian vowel it returns -1
        /// 0 - male, 1 - female
        /// 
        /// Тухайн char - ын эр эсвэл эм гэдгийг буцаана. Хэрэв тухайн char нь Монгол хэлний эгшиг биш бол -1 буцаана.
        /// 0 - эр, 1 - эм
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static int GetGender(this Char C)
        {
            if (C.GetLetterType() == letterType.Vowel)
            {
                if (C == 'а' || C == 'о' || C == 'у' || C == 'я' || C == 'ё') return 0;
                else return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Changes the letter to the best...
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static char ChangeLetter(this Char C, char A)
        {
            int indexA = 0, indexB = 0;
            if (A == 'а') { indexB = 0; indexA = 0; }
            if (A == 'о') { indexB = 1; indexA = 0; }
            if (A == 'у') { indexB = 2; indexA = 0; }
            if (A == 'я') { indexB = 3; indexA = 0; }
            if (A == 'ё') { indexB = 4; indexA = 0; }
            if (A == 'э') { indexB = 5; indexA = 1; }
            if (A == 'ө') { indexB = 6; indexA = 1; }
            if (A == 'ү') { indexB = 7; indexA = 1; }
            if (A == 'е') { indexB = 8; indexA = 1; }
            if (A == 'ю') { indexB = 9; indexA = 1; }
            if (A == 'и') { indexB = 10; indexA = 1; }
            if (A == 'й') { indexB = 11; indexA = 1; }
            if (_ruleArray[indexA, indexB] == '0') return A;
            else return _ruleArray[indexA, indexB];
        }
    }
}
