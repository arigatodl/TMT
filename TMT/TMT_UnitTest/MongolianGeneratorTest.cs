using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TMT_UnitTest
{
    using System.Text;
    using TMT.Rule;
    using TMT.Mongolian;
    using TMT;

    [TestClass]
    public class MongolianGeneratorTest
    {
        [TestMethod]
        public void GenerateTest()
        {
            string filePath = "..\\..\\TestFiles\\MongolianGenerator.txt";
            MongolianGenerator m = MongolianGenerator.Instance;
            Mongo.Instance.ConnectionString = "mongodb://localhost";
            Mongo.Instance.DatabaseName = "TMTDB";
            

            string[] Tests = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);
            foreach (string Test in Tests)
            {
                MongolianWord tempMongolianWord = new MongolianWord();
                tempMongolianWord.Word = Test.Split(' ')[0];
                m.Root = tempMongolianWord;
                m.Suffixes = new System.Collections.Generic.List<MongolianSuffix>();
                Console.Write(m.Root.Word + " ");
                for (int i = 1; i < Test.Split(' ').Length; i++)
                {
                    MongolianSuffix tempMongolianSuffix = new MongolianSuffix();
                    tempMongolianSuffix.Word = Test.Split(' ')[i];
                    m.Suffixes.Add(tempMongolianSuffix);
                    Console.Write(m.Suffixes[i - 1].Word + " ");
                }
                m.Generate();
                Console.WriteLine(m.ResultWord.Word);
            }
        }
    }
}
