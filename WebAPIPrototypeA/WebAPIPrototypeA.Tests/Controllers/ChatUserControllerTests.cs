using NUnit.Framework;
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
		private ChatUserControllerTests chatUserController { get; set; }
		private IRepository<ChatUser> chatUserRepository { get; set; }
		private IContext sessionContext { get; set; }
		private IApplicationSettings fakeApplicationSettings { get; set; }

		[SetUp]
		public void SetUp()
		{
			this.sessionContext  = new SessionStateContext();
			this.fakeApplicationSettings = new FakeApplicationSettings();
			this.chatUserRepository = new ChatUserRepository(this.sessionContext, this.fakeApplicationSettings);
			this.chatUserController = new ChatUserController(this.channelRepository, this.fakeApplicationSettings);
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
			Assert.AreEqual(0, this.sessionContext.ChatUsers.Count(), "the chat user list should be empty on start up");

			IHttpActionResult result = this.chatUserController.Save(user);
			Assert.AreEqual(1, this.sessionContext.ChatUsers.Count(), "the chatuser list was not updated");
			Assert.AreEqual("test user", this.sessionContext.ChatUsers.FirstOrDefault().UserName, "the chatuser name was incorrect");
			Assert.AreEqual(this.fakeApplicationSettings.TokenBase, this.sessionContext.ChatUsers.FirstOrDefault().UserToken, "the chatuser token was incorrect");
			Assert.AreEqual(typeof(System.Web.Http.Results.OkResult), result.GetType(), "the result was incorrect");
		}
	}
}
