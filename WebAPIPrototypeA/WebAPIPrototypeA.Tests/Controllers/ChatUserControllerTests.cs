﻿using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebAPIPrototypeA.Controllers;
using System.Web.Http.Results;
using System.Web;
using Models;
using Repository;

namespace WebAPIPrototypeA.Tests
{
	[TestFixture()]
	public class ChatUserControllerTests : ControllerTestBase
	{
		private ChatUserController chatUserController { get; set; }
		private IRepository<ChatUser> chatUserRepository { get; set; }
		private IContext sessionContext { get; set; }
		private IApplicationSettings fakeApplicationSettings { get; set; }

		[SetUp]
		public void SetUp()
		{
			this.sessionContext  = new SessionStateContext();
			this.fakeApplicationSettings = new FakeApplicationSettings();
			this.chatUserRepository = new ChatUserRepository(this.sessionContext, this.fakeApplicationSettings);
			this.chatUserController = new ChatUserController(this.chatUserRepository);
			HttpContext.Current = StaticHttpMock.FakeHttpContext("/test");
		}

		[TearDown]
		public void CleanUp()
		{
			this.chatUserController = null;
			this.sessionContext = null;
			this.fakeApplicationSettings = null;
			this.chatUserRepository = null;
			HttpContext.Current = null;
		}

		[Test()]
		public void SaveChatUser()
		{
			ChatUser user = new ChatUser { UserName = "test user" };
			Assert.AreEqual(0, this.sessionContext.ChatUsers.Count(), "the chat user list should be empty on start up");

			IHttpActionResult result = this.chatUserController.Save(user);
			Assert.AreEqual(1, this.sessionContext.ChatUsers.Count(), "the chatuser list was not updated");
			Assert.AreEqual("test user", this.sessionContext.ChatUsers.FirstOrDefault().UserName, "the chatuser name was incorrect");
			Assert.AreEqual(this.fakeApplicationSettings.TokenBase, this.sessionContext.ChatUsers.FirstOrDefault().UserToken, "the chatuser token was incorrect");
			Assert.AreEqual(typeof(System.Web.Http.Results.OkResult), result.GetType(), "the result was incorrect");
		}

		[Test()]
		public void SaveChatUserUnique()
		{
			HttpContext.Current.Session["ChatUsers"] = base.GetFakeChatUsers(this.fakeApplicationSettings);
			ChatUser user = new ChatUser { UserName = base.GetFakeChatUsers(this.fakeApplicationSettings).LastOrDefault().UserName };
			Assert.AreEqual(base.GetFakeChatUsers(this.fakeApplicationSettings).Count(), this.sessionContext.ChatUsers.Count(), "the chat user list should be empty on start up");

			StatusCodeResult result = this.chatUserController.Save(user) as StatusCodeResult;
			Assert.AreEqual(base.GetFakeChatUsers(this.fakeApplicationSettings).Count(), this.sessionContext.ChatUsers.Count(), "the chatuser list was updated!");
			Assert.AreEqual("test user", this.sessionContext.ChatUsers.FirstOrDefault().UserName, "the chatuser name was incorrect");
			Assert.AreEqual(this.fakeApplicationSettings.TokenBase, this.sessionContext.ChatUsers.FirstOrDefault().UserToken, "the chatuser token was incorrect");
			Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "the result was incorrect");
		}

		[Test()]
		public void GetAllAvailableChatUsers()
		{
			this.sessionContext.ChatUsers = base.GetFakeChatUsers(this.fakeApplicationSettings);
			OkNegotiatedContentResult<IEnumerable<ChatUser>> chatUsers = this.chatUserController.GetAllChatUsers() as OkNegotiatedContentResult<IEnumerable<ChatUser>>;

			Assert.AreEqual(this.sessionContext.ChatUsers.Count(), chatUsers.Content.Count(), "the returned chat users were not correct");
		}

		[Test()]
		public void GetChatUserByName()
		{
			this.sessionContext.ChatUsers = base.GetFakeChatUsers(this.fakeApplicationSettings);

			OkNegotiatedContentResult<IEnumerable<ChatUser>> chatUsers = this.chatUserController.GetChatUser("test user 1") as OkNegotiatedContentResult<IEnumerable<ChatUser>>;

			Assert.AreEqual(1, chatUsers.Content.Count(), "only one chat user should be returned by the query");
			Assert.AreEqual("test user 1", chatUsers.Content.FirstOrDefault().UserName, "the chat users were not returned properly");
		}

		[Test()]
		public void GetChannelByNameDoesntExist()
		{
			this.sessionContext.ChatUsers = base.GetFakeChatUsers(this.fakeApplicationSettings);
			Assert.AreEqual(0, this.sessionContext.ChatUsers.Count(x => x.UserName == "non-existing user"), "make sure test is properly primed");

			OkNegotiatedContentResult<IEnumerable<ChatUser>> chatUsers = this.chatUserController.GetChatUser("non-existing channel") as OkNegotiatedContentResult<IEnumerable<ChatUser>>;
			Assert.AreEqual(0, chatUsers.Content.Count(), "no channels should be returned by the query");
		}

	}
}