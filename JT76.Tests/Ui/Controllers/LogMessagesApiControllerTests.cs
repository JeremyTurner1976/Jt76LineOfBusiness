using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using JT76.Common.Services;
using JT76.Data.Database;
using JT76.Data.Database.ModelRepositories;
using JT76.Data.Factories;
using JT76.Data.Models;
using JT76.Ui;
using JT76.Ui.Controllers;
using JT76.Ui.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JT76.Tests.Ui.Controllers
{
    [TestClass]
    public class LogMessagesApiControllerTests
    {
        public Mock<JtDbContext> Context { get; set; }
        IUiService UiService { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            // Create a mock set and context
            Mock<DbSet<LogMessage>> testSet = new Mock<DbSet<LogMessage>>()
                .SetupData(JtMockFactory.GetLogMessageMocks().ToList());

            Context = new Mock<JtDbContext>();

            Context.Setup(c => c.LogMessages).Returns(testSet.Object);

            UiService =
                new UiService(
                    new DbLoggingService(new ErrorRepository(Context.Object),
                                        new LogMessageRepository(Context.Object)
                                        ),
                    new EmailLoggingService(new ConfigEmailService(new ConfigService(),
                                            new FileService())
                                            ),
                    new FileLoggingService(new FileService())
                    );
        }

        [TestCleanup]
        public void CleanUp()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void GetTest()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            var repository = new LogMessageRepository(Context.Object);
            var viewModel = new AdminViewModel(null, repository, UiService);

            //ensure test validity
            var testSet = repository.GetLogMessages();
            Assert.AreEqual(3, testSet.Count());

            //action
            var apiController = new LogMessagesApiController(viewModel, null);
            var result = apiController.Get();

            //ensure action
            var enumerable = result as IList<LogMessage> ?? result.ToList();
            Assert.AreEqual(enumerable.Count(), testSet.Count());
            foreach (var item in testSet)
            {
                int itemId = item.Id;
                var resultItem = testSet.FirstOrDefault(x => x.Id == itemId);
                Assert.AreEqual(resultItem, item);
            }
        }

        [TestMethod]
        public void PostTest()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            var repository = new LogMessageRepository(Context.Object);
            var viewModel = new AdminViewModel(null, repository, UiService);

            //ensure test validity
            string strGuid = Guid.NewGuid().ToString();
            Assert.AreEqual(3, repository.GetLogMessages().Count());

            var foundItem =
                repository.GetLogMessages().FirstOrDefault(x => x.StrLogMessage.Contains(strGuid));
            if (foundItem != null)
                Assert.Fail("Value already in the repository");


            //get the item to test against
            var createdItem = new LogMessage()
            {
                DtCreated = DateTime.UtcNow,
                Id = 0,
                StrLogMessage = "Test Log Message " + strGuid
            };

            //primary action
            var apiController = new LogMessagesApiController(viewModel, null) { Request = new HttpRequestMessage() };
            apiController.Request.SetConfiguration(new HttpConfiguration());

            var response = apiController.Post(createdItem);

            //ensure result content
            LogMessage contentValue;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            bool bResult = response.TryGetContentValue(out contentValue);

            if (bResult)
                Assert.AreEqual(createdItem, contentValue);
            else
                Assert.Fail("No response content found");


            //ensure action
            Assert.AreEqual(4, repository.GetLogMessages().Count());

            foundItem = repository.GetLogMessages().FirstOrDefault(x => x.StrLogMessage.Contains(strGuid));
            if (foundItem != null)
            {
                Assert.IsTrue(repository.ModelEquals(createdItem, foundItem));
            }
            else
                Assert.Fail("No test item found");
        }
    }
}