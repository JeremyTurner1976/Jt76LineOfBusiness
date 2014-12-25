using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using JT76.Data.Database;
using JT76.Data.Database.ModelRepositories;
using JT76.Data.Factories;
using JT76.Data.Models;
using JT76.Ui.Controllers;
using JT76.Ui.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JT76.Tests.Ui.Controllers
{
    [TestClass]
    public class ErrorsApiControllerTests
    {
        public Mock<JtDbContext> Context { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            // Create a mock set and context
            Mock<DbSet<Error>> errorSet = new Mock<DbSet<Error>>()
                .SetupData(JtMockFactory.GetErrorMocks().ToList());

            Context = new Mock<JtDbContext>();

            Context.Setup(c => c.Errors).Returns(errorSet.Object);
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

            var errorRepository = new ErrorRepository(Context.Object);
            var viewModel = new AdminViewModel(errorRepository, null, null);

            //ensure test validity
            IQueryable<Error> errorSet = errorRepository.GetErrors();
            Assert.AreEqual(3, errorSet.Count());

            //action
            var apiController = new ErrorsApiController(viewModel, null);
            var result = apiController.Get();

            //ensure action
            IList<Error> enumerable = result as IList<Error> ?? result.ToList();
            Assert.AreEqual(enumerable.Count(), errorSet.Count());
            foreach (Error item in errorSet)
            {
                int itemId = item.Id;
                Error resultItem = errorSet.FirstOrDefault(x => x.Id == itemId);
                Assert.AreEqual(resultItem, item);
            }
        }

        [TestMethod]
        public void PostTest()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            Error createdError = null;

            var errorRepository = new ErrorRepository(Context.Object);
            var viewModel = new AdminViewModel(errorRepository, null, null);

            //ensure test validity
            string strGuid = Guid.NewGuid().ToString();
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
                //get the error to test against
                createdError = ErrorFactory.GetErrorFromException(e, ErrorLevels.Message, "LogErrorTest" + strGuid);

                //primary action
                var apiController = new ErrorsApiController(viewModel, null) {Request = new HttpRequestMessage()};
                apiController.Request.SetConfiguration(new HttpConfiguration());

                HttpResponseMessage response = apiController.Post(createdError);

                //ensure result content
                Error contentValue;

                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                bool bResult = response.TryGetContentValue(out contentValue);

                if (bResult)
                    Assert.AreEqual(createdError, contentValue);
                else
                    Assert.Fail("No response content found");
            }

            //ensure action
            Assert.AreEqual(4, errorRepository.GetErrors().Count());

            foundError = errorRepository.GetErrors().FirstOrDefault(x => x.StrAdditionalInformation.Contains(strGuid));
            if (createdError != null && foundError != null)
            {
                Assert.IsTrue(errorRepository.ModelEquals(createdError, foundError));
            }
            else
                Assert.Fail("No test error found");
        }
    }
}