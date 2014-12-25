using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using JT76.Common.Services;
using JT76.Data.Database;
using JT76.Data.Factories;
using JT76.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JT76.Tests.Common.Services
{
    [TestClass]
    public class EmailLoggingServiceTests
    {
        private  string _strGuid;
        private ConfigEmailService _emailService;
        private int _currentFileCount;
        private string _emailPickupDirectory;
        private FileService _fileService;

        [TestInitialize]
        public void Initialize()
        {
            _fileService = new FileService();
            _emailPickupDirectory = _fileService.GetDirectoryFolderLocation(DirectoryFolders.Jt76Email);
            _currentFileCount = Directory.GetFiles(_emailPickupDirectory).Count();
            _emailService = new ConfigEmailService(new ConfigService(), _fileService);
            _strGuid = Guid.NewGuid().ToString();
        }

        [TestCleanup]
        public void CleanUp()
        {
        }

        [TestMethod]
        public void LogErrorTest()
        {
            var currentFileCount = Directory.GetFiles(_emailPickupDirectory).Count();
            var emailLoggingService = new EmailLoggingService(_emailService);

            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                bool bResult = emailLoggingService.LogError(e, ErrorLevels.Message, "LogErrorTest " + _strGuid);
                Assert.IsTrue(bResult);
            }

            //for testing the async call (ConfigEmailService tests the await call)
            //allows threads time to complete before this test instance is closed
            //not a concern on a live environment
            Thread.Sleep(5 * 1000);//5 seconds sleep

            Assert.AreEqual(Directory.GetFiles(_emailPickupDirectory).Count(), currentFileCount + 1);
        }

        [TestMethod]
        public void LogMessageTest()
        {
            var currentFileCount = Directory.GetFiles(_emailPickupDirectory).Count();
            var emailLoggingService = new EmailLoggingService(_emailService);

            bool bResult = emailLoggingService.LogMessage("LogMessageTest " + _strGuid);
            Assert.IsTrue(bResult);

            //for testing the async call (ConfigEmailService tests the await call)
            //allows threads time to complete before this test instance is closed
            //not a concern on a live environment
            Thread.Sleep(5 * 1000);//5 seconds sleep

            Assert.AreEqual(Directory.GetFiles(_emailPickupDirectory).Count(), currentFileCount + 1);
        }
    }
}