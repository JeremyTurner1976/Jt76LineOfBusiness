using System;
using System.Data.Entity;
using System.Linq;
using JT76.Common.Services;
using JT76.Data.Database;
using JT76.Data.Database.ModelRepositories;
using JT76.Data.Factories;
using JT76.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JT76.Tests.Common.Services
{
    [TestClass]
    public class DbLoggingServiceTests
    {
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
        }

        [TestCleanup]
        public void CleanUp()
        {
        }

        [TestMethod]
        public void LogErrorTest()
        {
            Error createdError = null;

            var errorRepository = new ErrorRepository(Context.Object);
            var logRepository = new LogMessageRepository(Context.Object);

            var loggingService = new DbLoggingService(errorRepository, logRepository);
            string strGuid = Guid.NewGuid().ToString();

            //ensure test validity
            Assert.AreEqual(3, errorRepository.GetErrors().Count());

            Error foundError =
                errorRepository.GetErrors().FirstOrDefault(x => x.StrAdditionalInformation.Contains(strGuid));
            if (foundError != null)
                Assert.Fail("Value already in the repository");

            //action
            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                createdError = ErrorFactory.GetErrorFromException(e, ErrorLevels.Message, "LogErrorTest" + strGuid);
                bool result = loggingService.LogError(e, ErrorLevels.Message, "LogErrorTest" + strGuid);
                Assert.IsTrue(result);
            }

            //ensure action
            Assert.AreEqual(4, errorRepository.GetErrors().Count());

            foundError = errorRepository.GetErrors().FirstOrDefault(x => x.StrAdditionalInformation.Contains(strGuid));
            if (createdError != null && foundError != null)
            {
                if (!errorRepository.ModelEquals(createdError, foundError))
                    Assert.Fail("Non Equal Objects");
            }
            else
                Assert.Fail("No test error found");
        }

        [TestMethod]
        public void LogMessageTest()
        {
            var errorRepository = new ErrorRepository(Context.Object);
            var logRepository = new LogMessageRepository(Context.Object);

            var loggingService = new DbLoggingService(errorRepository, logRepository);

            //ensure test validity
            string strGuid = Guid.NewGuid().ToString();
            Assert.AreEqual(3, logRepository.GetLogMessages().Count());

            LogMessage foundLogMessage =
                logRepository.GetLogMessages().FirstOrDefault(x => x.StrLogMessage.Contains(strGuid));
            if (foundLogMessage != null)
                Assert.Fail("Value already in the repository");

            var createdLogMessage = new LogMessage
            {
                DtCreated = DateTime.UtcNow,
                Id = 0,
                StrLogMessage = "Test Log Message " + strGuid
            };
            bool result = loggingService.LogMessage("Test Log Message " + strGuid);
            Assert.IsTrue(result);

            //ensure action
            Assert.AreEqual(4, logRepository.GetLogMessages().Count());

            foundLogMessage = logRepository.GetLogMessages().FirstOrDefault(x => x.StrLogMessage.Contains(strGuid));
            if (foundLogMessage != null)
            {
                if (!logRepository.ModelEquals(foundLogMessage, createdLogMessage))
                    Assert.Fail("Non Equal Objects for Object Entry");
            }
            else
                Assert.Fail("No test logMessage found");
        }
    }
}