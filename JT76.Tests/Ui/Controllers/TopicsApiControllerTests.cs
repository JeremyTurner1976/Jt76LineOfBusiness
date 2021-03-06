﻿using System;
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
    public class TopicsApiControllerTests
    {
        public Mock<JtDbContext> Context { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            // Create a mock set and context
            var testSet = new Mock<DbSet<Topic>>()
                .SetupData(JtMockFactory.GetTopicMocks().ToList());

            var replyMocks = (from item in JtMockFactory.GetTopicMocks()
                              select item)
                                            .TakeWhile(x => x.Replies != null)
                                            .SelectMany(x => x.Replies).ToList();

            // Create a mock set and context
            var testReplySet = new Mock<DbSet<Reply>>()
                .SetupData(replyMocks);

            Context = new Mock<JtDbContext>();

            Context.Setup(c => c.Topics).Returns(testSet.Object);
            Context.Setup(c => c.Replies).Returns(testReplySet.Object);
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

            var repository = new TopicRepository(Context.Object);
            var replyRepository = new ReplyRepository(Context.Object);
            var viewModel = new MessageBoardViewModel(repository, replyRepository);

            //ensure test validity
            var testSet = repository.GetTopicsIncludingReplies();
            Assert.AreEqual(4, testSet.Count());

            //action
            var apiController = new TopicsApiController(viewModel, null);
            var result = apiController.Get();

            //ensure action
            var enumerable = result as IList<Topic> ?? result.ToList();
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

            var repository = new TopicRepository(Context.Object);
            var replyRepository = new ReplyRepository(Context.Object);
            var viewModel = new MessageBoardViewModel(repository, replyRepository);

            //ensure test validity
            var testSet = repository.GetTopicsIncludingReplies();
            Assert.AreEqual(4, testSet.Count());

            var strGuid = Guid.NewGuid().ToString();
            var foundItem =
                repository.GetTopicsIncludingReplies().FirstOrDefault(x => x.StrBody.Contains(strGuid));
            if (foundItem != null)
                Assert.Fail("Value already in the repository");


            //get the item to test against
            var createdItem = new Topic()
            {
                StrTitle = Faker.Lorem.Sentence(),
                DtCreated = DateTime.UtcNow,
                Id = 0,
                StrBody = "Test Topic " + strGuid,
                Replies = new List<Reply>()
                {
                    new Reply()
                    {
                        DtCreated = DateTime.UtcNow,
                        Id = 0,
                        StrBody = "Test Reply " + strGuid,
                        TopicId = 0
                    }
                }
            };

            //primary action
            var apiController = new TopicsApiController(viewModel, null) { Request = new HttpRequestMessage() };
            apiController.Request.SetConfiguration(new HttpConfiguration());

            var response = apiController.Post(createdItem);

            //ensure result content
            Topic contentValue;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            bool bResult = response.TryGetContentValue(out contentValue);

            if (bResult)
                Assert.AreEqual(createdItem, contentValue);
            else
                Assert.Fail("No response content found");

            //ensure action
            Assert.AreEqual(5, repository.GetTopicsIncludingReplies().Count());

            foundItem = repository.GetTopicsIncludingReplies().FirstOrDefault(x => x.StrBody.Contains(strGuid));
            if (foundItem != null)
            {
                Assert.IsTrue(repository.ModelEquals(createdItem, foundItem));
            }
            else
                Assert.Fail("No test item found");
        }
    }
}