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
    }
}
