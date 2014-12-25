using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JT76.Data.Factories;
using JT76.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JT76.Tests.Data.Factories
{
    [TestClass]
    public class JtMockFactoryTests
    {
        [TestMethod]
        public void GetErrorMocksTest()
        {
            var mocks = JtMockFactory.GetErrorMocks();
            Assert.IsTrue(mocks.Any());

            Error mockItem = mocks.First();

            Assert.IsTrue(mockItem != null);
            Debug.Assert(mockItem != null, "mockItem != null");
            Assert.IsTrue(mockItem.HasNoEmptyStrings());
        }

        [TestMethod]
        public void GetLogMessageMocksTest()
        {
            var mocks = JtMockFactory.GetLogMessageMocks();
            Assert.IsTrue(mocks.Any());

            LogMessage mockItem = mocks.First();

            Assert.IsTrue(mockItem != null);
            Debug.Assert(mockItem != null, "mockItem != null");
            Assert.IsTrue(mockItem.HasNoEmptyStrings());
        }

        [TestMethod]
        public void GetTopicMocksTest()
        {
            var mocks = JtMockFactory.GetTopicMocks();
            Assert.IsTrue(mocks.Any());

            Topic mockItem = mocks.First();

            Assert.IsTrue(mockItem != null);
            Debug.Assert(mockItem != null, "mockItem != null");
            Assert.IsTrue(mockItem.HasNoEmptyStrings());

            Topic mockReply = mocks.TakeWhile(x => x.Replies != null && x.Replies.Any()).First();
            Assert.IsTrue(mockReply != null);
            Debug.Assert(mockReply != null, "mockReply != null");
            Assert.IsTrue(mockReply.HasNoEmptyStrings());
        }

        [TestMethod]
        public void GetFakerParagraphsTest()
        {
            const int nCount = 4;
            string strMock = JtMockFactory.GetFakerParagraphs(nCount);

            string[] mockItems = strMock.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);

            Assert.IsTrue(mockItems.Count() == nCount);
            foreach (string item in mockItems)
                Assert.IsTrue(!string.IsNullOrEmpty(item));
        }
    }
}