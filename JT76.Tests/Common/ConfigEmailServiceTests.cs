using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JT76.Common.Services;
using JT76.Data.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JT76.Tests.Common
{
    [TestClass]
    public class ConfigEmailServiceTests
    {
        [TestMethod]
        public void SendMailTest()
        {
            var fileService = new FileService();
            var emailPickupDirectory = fileService.GetDirectoryFolderLocation(DirectoryFolders.Jt76Email);
            var currentFileCount = Directory.GetFiles(emailPickupDirectory).Count();

            bool[] result;
            var configEmailService = new ConfigEmailService(new ConfigService(), fileService);
            try
            {
                result =
                    configEmailService.SendMail("363015fdfa2f4211b9d42ee5cf@gmail.com",
                        "JT76 MAIL", "This is coming from SendMailTest() a unit test. Hope things are well").Result;

            }
            catch (AggregateException ae)
            {
                try
                {
                    var errorString = ErrorFactory.GetAggregateErrorAsString(ae);
                    fileService.SaveTextToDirectoryFile(DirectoryFolders.Jt76Email, errorString);
                }
                catch { }

                throw ae.Flatten();
            }

            //for testing the async call (awaiting result in this test instead, EmailLoggingService tests do not await the result)
            //allows threads time to complete before this test instance is closed
            //not a concern on a live environment
            //Thread.Sleep(15 * 1000);//15 seconds sleep

            //visual confirmation of email receipt is required
            Assert.IsTrue(!result.Contains(false));
            Assert.AreEqual(Directory.GetFiles(emailPickupDirectory).Count(), currentFileCount + 1);
        }

        [TestMethod]
        public void SendMeMailTest()
        {
            var fileService = new FileService();
            var emailPickupDirectory = fileService.GetDirectoryFolderLocation(DirectoryFolders.Jt76Email);
            var currentFileCount = Directory.GetFiles(emailPickupDirectory).Count();

            bool[] result;
            var configEmailService = new ConfigEmailService(new ConfigService(), fileService);
            try
            {
                result =
                    configEmailService.SendMeMail("This is coming from SendMeMail() a unit test. Hope things are going twice as well as before").Result;

            }
            catch (AggregateException ae)
            {
                try
                {
                    var errorString = ErrorFactory.GetAggregateErrorAsString(ae);
                    fileService.SaveTextToDirectoryFile(DirectoryFolders.Jt76Email, errorString);
                }
                catch { }

                throw ae.Flatten();
            }

            //for testing the async call (awaiting result in this test instead, EmailLoggingService tests do not await the result)
            //allows threads time to complete before this test instance is closed
            //not a concern on a live environment
            //Thread.Sleep(15 * 1000);//15 seconds sleep

            //visual confirmation of email receipt is required
            Assert.IsTrue(!result.Contains(false));
            Assert.AreEqual(Directory.GetFiles(emailPickupDirectory).Count(), currentFileCount + 1);
        }
    }
}