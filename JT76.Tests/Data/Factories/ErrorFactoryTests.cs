using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JT76.Common.ObjectExtensions;
using JT76.Data.Factories;
using JT76.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JT76.Tests.Data.Factories
{
    [TestClass]
    public class ErrorFactoryTests
    {
        [TestMethod]
        public void GetErrorFromExceptionTest()
        {
            string strMessage = "This is a created error " + Guid.NewGuid();

            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                //expected exception
                Error error = ErrorFactory.GetErrorFromException(e, ErrorLevels.Critical, strMessage);

                Assert.IsTrue(error.Id == 0);
                TimeSpan diff = error.DtCreated - DateTime.UtcNow;
                Assert.IsTrue(diff.Ticks <= 100);
                Assert.IsTrue(error.StrAdditionalInformation.Contains(strMessage));
                string strEnum = ErrorLevels.Critical.ToNameString();
                Assert.IsTrue(strEnum != null && error.StrErrorLevel.Contains(strEnum));
                Assert.IsTrue(error.StrMessage.Equals("Attempted to divide by zero."));
                Assert.IsTrue(error.StrSource.Contains("JT76.Data") && error.StrSource.Contains("ErrorFactory"));
                Assert.IsTrue(error.StrStackTrace.Contains("GetErrorFromExceptionTest"));
            }

            try
            {
                var failedMethod = ErrorFactory.GetThrownAggregateException();
                Assert.Fail();
            }
            catch (AggregateException ae)
            {
                var flattened = ae.Flatten();

                //expected exception
                Error error = ErrorFactory.GetErrorFromException(flattened, ErrorLevels.Critical, strMessage);

                Assert.IsTrue(error.Id == 0);
                TimeSpan diff = error.DtCreated - DateTime.UtcNow;
                Assert.IsTrue(diff.Ticks <= 100);
                Assert.IsTrue(error.StrAdditionalInformation.Contains(strMessage));
                string strEnum = ErrorLevels.Critical.ToNameString();
                Assert.IsTrue(strEnum != null && error.StrErrorLevel.Contains(strEnum));
                Assert.IsTrue(error.StrMessage.Contains("One or more errors occurred."));
                Assert.IsTrue(error.StrStackTrace.Contains(@"Access to the path 'C:\Users\All Users\Application Data' is denied."));
                Assert.IsTrue(error.StrStackTrace.Contains(@"Data\Factories\ErrorFactory.cs"));
            }
        }

        [TestMethod]
        public void GetErrorAsHtmlTest()
        {
            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                //expected exception
                string error = ErrorFactory.GetErrorAsHtml(e);
                Assert.IsTrue(error.Contains("Attempted to divide by zero."));
                Assert.IsTrue(error.Contains("JT76.Data") && error.Contains("ErrorFactory"));
                Assert.IsTrue(error.Contains("ErrorFactoryTests.cs:"));
            }

            try
            {
                var failedMethod = ErrorFactory.GetThrownAggregateException();
                Assert.Fail();
            }
            catch (AggregateException ae)
            {
                var flattened = ae.Flatten();

                //expected exception
                var error = ErrorFactory.GetErrorAsHtml(flattened);
                Assert.IsTrue(error.Contains("One or more errors occurred."));
                Assert.IsTrue(error.Contains(@"Access to the path 'C:\Users\All Users\Application Data' is denied."));
                Assert.IsTrue(error.Contains(@"Data\Factories\ErrorFactory.cs"));
            }
        }

        [TestMethod]
        public void GetErrorAsStringTest()
        {
            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                //expected exception
                string error = ErrorFactory.GetErrorAsString(e);
                Assert.IsTrue(error.Contains("Attempted to divide by zero."));
                Assert.IsTrue(error.Contains("JT76.Data") && error.Contains("ErrorFactory"));
                Assert.IsTrue(error.Contains("ErrorFactoryTests.cs:"));
            }

            try
            {
                var failedMethod = ErrorFactory.GetThrownAggregateException();
                Assert.Fail();
            }
            catch (AggregateException ae)
            {
                var flattened = ae.Flatten();

                //expected exception
                var error = ErrorFactory.GetErrorAsString(flattened);
                Assert.IsTrue(error.Contains("One or more errors occurred."));
                Assert.IsTrue(error.Contains(@"Access to the path 'C:\Users\All Users\Application Data' is denied."));
                Assert.IsTrue(error.Contains(@"Data\Factories\ErrorFactory.cs"));
            }
        }

        [TestMethod()]
        public void GetAggregateErrorAsStringTest()
        {
            try
            {
                var failedMethod = ErrorFactory.GetThrownAggregateException();
                Assert.Fail();
            }
            catch (AggregateException ae)
            {
                    var flattened = ae.Flatten();

                    //expected exception
                    var error = ErrorFactory.GetAggregateErrorAsString(ae);
                    Assert.IsTrue(error.Contains("One or more errors occurred."));
                    Assert.IsTrue(error.Contains(@"Access to the path 'C:\Users\All Users\Application Data' is denied."));
                    Assert.IsTrue(error.Contains("Index was outside the bounds of the array."));
                    Assert.IsTrue(error.Contains("JT76.Tests") && error.Contains("ErrorFactoryTests"));
            }
        }

        [TestMethod]
        public void GetThrownExceptionTest()
        {
            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException)
            {
                //expected exception
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void GetThrownAggregateExceptionTest()
        {
            try
            {
                ErrorFactory.GetThrownAggregateException();
                Assert.Fail();
            }
            catch (AggregateException)
            {
                //expected exception
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}