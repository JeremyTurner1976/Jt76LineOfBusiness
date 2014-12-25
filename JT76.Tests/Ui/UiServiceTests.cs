using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using JT76.Common.Services;
using JT76.Data.Database;
using JT76.Data.Database.ModelRepositories;
using JT76.Data.Factories;
using JT76.Data.Models;
using JT76.Ui;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JT76.Tests.Ui
{
    [TestClass]
    public class UiServiceTests
    {
        //Facade Pattern - Provides a unified interface to a set of interfaces in a subsystem
        //as such no functionality is tested only expecting a true result and no exceptions

        private FileService _fileService;
        private ConfigEmailService _emailService;
        private ConfigService _configService;
        private EmailLoggingService _emailLoggingService;
        private FileLoggingService _fileLoggingService;
        private ErrorRepository _errorRepository;
        private LogMessageRepository _logMessageRepository;
        private DbLoggingService _dbLoggingService;
        public Mock<JtDbContext> Context { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            // Create a mock set and context
            Mock<DbSet<Error>> errorSet = new Mock<DbSet<Error>>()
                .SetupData(JtMockFactory.GetErrorMocks().ToList());

            // Create a mock set and context
            Mock<DbSet<LogMessage>> logMessageSet = new Mock<DbSet<LogMessage>>()
                .SetupData(JtMockFactory.GetLogMessageMocks().ToList());

            Context = new Mock<JtDbContext>();

            Context.Setup(c => c.Errors).Returns(errorSet.Object);
            Context.Setup(c => c.LogMessages).Returns(logMessageSet.Object);

            //Email Logger
            _configService  = new ConfigService();
            _fileService  = new FileService();
            _emailService = new ConfigEmailService(_configService, _fileService);
            _emailLoggingService = new EmailLoggingService(_emailService);

            //File Logger
            _fileLoggingService = new FileLoggingService(_fileService);

            //Db Logger
            _errorRepository = new ErrorRepository(Context.Object);
            _logMessageRepository = new LogMessageRepository(Context.Object);
            _dbLoggingService = new DbLoggingService(_errorRepository, _logMessageRepository);
        }

        [TestCleanup]
        public void CleanUp()
        {
        }

        [TestMethod]
        public void ParseErrorAsHtmlTest()
        {
            var uiService = new UiService(_dbLoggingService, _emailLoggingService, _fileLoggingService);

            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (Exception e)
            {
                var strHtml = uiService.ParseErrorAsHtml(e);
                Assert.IsTrue(!string.IsNullOrEmpty(strHtml));
            }
        }

        [TestMethod]
        public void LogMessageTest()
        {
            var uiService = new UiService(_dbLoggingService, _emailLoggingService, _fileLoggingService);
            const string strLogMessage = "This is a leg message comming from the UiService.";

            var bResult = uiService.LogMessage(strLogMessage);
            Assert.IsTrue(bResult);
        }

        [TestMethod]
        public void HandleErrorTest()
        {
            var uiService = new UiService(_dbLoggingService, _emailLoggingService, _fileLoggingService);

            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (Exception e)
            {
                var result = uiService.HandleError(e);
                Assert.IsTrue(result);
            }
        }

        [TestMethod()]
        public void SendMeMailTest()
        {
            var uiService = new UiService(_dbLoggingService, _emailLoggingService, _fileLoggingService);
            const string strLogMessage = "This is a leg message comming from the UiService.";

            uiService.SendMeMail(strLogMessage);

            //visually ensure you got the mail, this is being sent to the PrimaryDeveloperEmail only
        }
    }
}