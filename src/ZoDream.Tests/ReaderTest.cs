using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoDream.Shared.Readers.Angular;

namespace ZoDream.Tests
{
    [TestClass]
    public class ReaderTest
    {
        [TestMethod]
        public void TestXlf()
        {
            var reader = new XlfReader();
            var package = reader.ReadAsync("D:\\Documents\\GitHub\\Angular-ZoDream\\src\\locale\\messages.xlf").GetAwaiter().GetResult();
            Assert.AreEqual(package.Language.Code, "en-US");
        }
    }
}