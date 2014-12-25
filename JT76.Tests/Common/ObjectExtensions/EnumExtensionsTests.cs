using JT76.Common.ObjectExtensions;
using JT76.Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JT76.Tests.Common.ObjectExtensions
{
    [TestClass()]
    public class EnumExtensionsTests
    {
        [TestMethod()]
        public void ToNameStringTest()
        {
            const DirectoryFolders enumItem = DirectoryFolders.Jt76Test;
            const string strCompareString = "Jt76Test";

            Assert.AreEqual(enumItem.ToNameString(), strCompareString);
        }
    }
}
