using System;
using System.Collections.Generic;
using System.Linq;
using Faker;
using JT76.Data.Factories;
using JT76.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JT76.Tests.Data.Abstract
{
    [TestClass]
    public class ModelBaseTests
    {
        [TestMethod]
        public void ForceValidDataTest()
        {
            string strMessage = "ForceValidDataTest() Message - " + Guid.NewGuid();

            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                //expected exception
                Error error = ErrorFactory.GetErrorFromException(e, ErrorLevels.Critical, strMessage);

                error.StrAdditionalInformation += Lorem.Paragraph(100);
                error.StrErrorLevel += Lorem.Paragraph(100);
                error.StrMessage += Lorem.Paragraph(100);
                error.StrSource += Lorem.Paragraph(100);
                error.StrStackTrace += Lorem.Paragraph(100);

                var errorMockLengths = new Queue<int>();
                errorMockLengths.Enqueue(error.StrAdditionalInformation.Length);
                errorMockLengths.Enqueue(error.StrErrorLevel.Length);
                errorMockLengths.Enqueue(error.StrMessage.Length);
                errorMockLengths.Enqueue(error.StrSource.Length);
                //errorMockLengths.Enqueue(error.StrStackTrace.Length); //4000 allowed

                //ensure a valid set
                Assert.IsTrue(errorMockLengths.All(x => x > 3000));

                //action
                error = (Error) error.ForceValidData();

                //ensure action
                errorMockLengths = new Queue<int>();
                errorMockLengths.Enqueue(error.StrAdditionalInformation.Length);
                errorMockLengths.Enqueue(error.StrErrorLevel.Length);
                errorMockLengths.Enqueue(error.StrMessage.Length);
                errorMockLengths.Enqueue(error.StrSource.Length);
                Assert.IsTrue(errorMockLengths.All(x => x < 2500));
            }
        }

        [TestMethod]
        public void HasNoEmptyStringsTest()
        {
            string strMessage = "HasNoEmptyStringsTest() Message - " + Guid.NewGuid();

            try
            {
                ErrorFactory.GetThrownException();
                Assert.Fail();
            }
            catch (DivideByZeroException e)
            {
                //expected exception
                Error error = ErrorFactory.GetErrorFromException(e, ErrorLevels.Critical, strMessage);

                error.StrAdditionalInformation += Lorem.Words(5);
                error.StrErrorLevel += Lorem.Words(5);
                error.StrMessage += Lorem.Words(5);
                error.StrSource += Lorem.Words(5);
                error.StrStackTrace += Lorem.Words(5);

                //ensure a valid set
                var errorMockLengths = new Queue<string>();
                errorMockLengths.Enqueue(error.StrAdditionalInformation);
                errorMockLengths.Enqueue(error.StrErrorLevel);
                errorMockLengths.Enqueue(error.StrMessage);
                errorMockLengths.Enqueue(error.StrSource);
                errorMockLengths.Enqueue(error.StrStackTrace);

                Assert.IsTrue(errorMockLengths.All(x => x.Length > 1));

                //action
                bool result = error.HasNoEmptyStrings();
                Assert.IsTrue(result);

                //add an empty string
                error.StrStackTrace = "";

                //action
                result = error.HasNoEmptyStrings();
                Assert.AreEqual(false, result);
            }
        }
    }
}