namespace TMT.Mongolian
{
    using System;

    public class MongolianWord
    {
        public enum gender
        {
            Male,   // Эр
            Female, // Эм
            Unknown // Мэдэгдэхгүй
        };

        private string _word;   // Үг
        private gender _gender; // Эр, эм

        /// <summary>
        /// Default constructor
        /// </summary>
        public MongolianWord()
        {
        }

        /// <summary>
        /// Gets and sets the Word
        /// 
        /// Үгийг авах болон тэмдэглэх
        /// </summary>
        public string Word
        {
            get
            {
                return _word;
            }
            set
            {
                _word = value;
                checkGender();  // Эр, эмийг олно.
            }
        }

        /// <summary>
        /// Gets and sets the Gender
        /// 
        /// Хүйсийг авах болон тэмдэглэх
        /// </summary>
        public gender Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                _gender = value;
            }
        }

        /// <summary>
        /// Returns the gender of the word based on Mongolian rule.
        /// If the gender of the word can not be determined it returns -1.
        /// 
        /// Үгийн эр болон эмийг Монгол хэлний эр эм үг бүтэх дүрмийн дагуу тодорхойлон буцаана.
        /// Хэрэв үгийн эр болон эмийг тодорхойлох боломжгүй бол -1 буцаана.
        /// Хэлний угт бүтэх а, о, у, я, ё, ю(у), ы долоог эр эгшиг гэнэ.
        /// Хэлний дунд бүтэх э, ө, ү, е, ю(ү) тавыг эм эгшиг гэнэ.
        /// Хэлний үзүүрт бүтэх и, й хоёрыг саармаг эгшиг гэнэ. Эр эгшиг орж бүтсэн үгийг эр үг гэнэ. Эм эгшиг орж бүтсэн үгийг эм үг гэнэ.
        /// Саармаг эгшгээр бүтсэн үгэнд залгавар залгахад эм эгшиг гардаг учир саармаг эгшгээр бүтсэн үгийг эм үгэнд тооцно. Жишээлбэл: ишиг-ишгээр, жил-жилүүд, чийг-чийгтэй гэх мэт.
        /// </summary>
        /// <returns></returns>
        public gender checkGender()
        {
            /// Finding the last vowel to define the gender of the word
            /// 
            /// Үгийн сүүлийн эгшгийг олж байна.
            for (int i = Word.Length - 1; i >= 0; i--)
            {
                int tempValue = Word[i].GetGender();
                if (tempValue != -1)
                {
                    if (tempValue == 0) { Gender = gender.Male; return gender.Male; }
                    else if (tempValue == 1) { Gender = gender.Female; return gender.Female; }
                }
            }

            Gender = gender.Unknown;
            return gender.Unknown;
        }
    }
}
