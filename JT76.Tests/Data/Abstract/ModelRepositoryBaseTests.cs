using System;
using System.Data.Entity;
using System.Linq;
using JT76.Data.Database;
using JT76.Data.Database.ModelRepositories;
using JT76.Data.Factories;
using JT76.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JT76.Tests.Data.Abstract
{
    [TestClass]
    public class ModelRepositoryBaseTests
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
        public void ModelEqualsTest()
        {
            Error createdError = null;
            Error createdErrorTwo = null;

            var errorRepository = new ErrorRepository(Context.Object);
            string strGuid = Guid.NewGuid().ToString();

            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                createdError = ErrorFactory.GetErrorFromException(e, ErrorLevels.Message, "LogErrorTest" + strGuid);
            }

            //ensure validity
            Error copiedError = createdError;

            //action
            Assert.AreEqual(createdError, copiedError);
            bool result = errorRepository.ModelEquals(createdError, copiedError);
            Assert.IsTrue(result);

            strGuid = Guid.NewGuid().ToString();
            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                createdErrorTwo = ErrorFactory.GetErrorFromException(e, ErrorLevels.Message, "LogErrorTest" + strGuid);
            }
            //ensure validity
            Assert.AreNotEqual(createdError, createdErrorTwo);

            //action
            result = errorRepository.ModelEquals(createdError, createdErrorTwo);
            Assert.AreEqual(false, result);
        }
    }
}