using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TMT_UnitTest
{
    using TMT.Mongolian;
    [TestClass]
    public class MongolianWordTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            //Mongolian
            MongolianWord m = new MongolianWord();
            m.Word = "Test";
            m.checkGender();
        }
    }
}
