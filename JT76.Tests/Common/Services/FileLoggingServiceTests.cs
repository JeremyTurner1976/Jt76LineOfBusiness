using System;
using JT76.Common.Services;
using JT76.Data.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JT76.Tests.Common.Services
{
    [TestClass]
    public class FileLoggingServiceTests
    {
        [TestMethod]
        public void LogErrorTest()
        {
            var fileService = new FileService();
            var fileLoggingService = new FileLoggingService(fileService);
            string strGuid = Guid.NewGuid().ToString();

            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                bool result = fileLoggingService.LogError(e, ErrorLevels.Message, "LogErrorTest " + strGuid);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void LogMessageTest()
        {
            var fileService = new FileService();
            var fileLoggingService = new FileLoggingService(fileService);
            string strGuid = Guid.NewGuid().ToString();

            bool result = fileLoggingService.LogMessage("LogMessageTest " + strGuid);
            Assert.IsTrue(result);
        }
    }
}