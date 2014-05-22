using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TMT_UnitTest
{
    using System.Text;
    using System.Collections.Generic;
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
                Console.Write(Test.Split(' ')[0] + " ");
                List<string> temp = new List<string>();
                for (int i = 1; i < Test.Split(' ').Length; i++)
                {
                    temp.Add(Test.Split(' ')[i]);
                    Console.Write(Test.Split(' ')[i] + " ");
                }
                m.Generate(Test.Split(' ')[0],temp);
                Console.WriteLine(m.ResultWord.Word);
            }
        }
    }
}
