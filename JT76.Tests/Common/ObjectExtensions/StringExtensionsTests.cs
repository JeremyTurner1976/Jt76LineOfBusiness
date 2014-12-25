using System;
using System.Collections.Generic;
using System.Linq;
using JT76.Common.ObjectExtensions;
using JT76.Data.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JT76.Tests.Common.ObjectExtensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void SplitOnNewLinesTest()
        {
            const int nCount = 4;
            string strMock = JtMockFactory.GetFakerParagraphs(nCount);

            IEnumerable<string> mockItems = strMock.SplitOnNewLines();
            IList<string> enumerable = mockItems as IList<string> ?? mockItems.ToList();

            Assert.IsTrue(enumerable.Count() == nCount);
            foreach (string item in enumerable)
                Assert.IsTrue(!string.IsNullOrEmpty(item));
        }

        [TestMethod]
        public void SplitOnBreaksTest()
        {
            const int nCount = 5;

            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                string strMock = ErrorFactory.GetErrorAsHtml(e);

                IEnumerable<string> mockItems = strMock.SplitOnBreaks();
                IList<string> enumerable = mockItems as IList<string> ?? mockItems.ToList();

                Assert.IsTrue(enumerable.Count() >= nCount);
                foreach (string item in enumerable)
                    Assert.IsTrue(!string.IsNullOrEmpty(item));
            }
        }
    }
}