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
	[WebApiTestClass]
	public class ChatUserControllerTests : ControllerTestBase
	{
		private ChatUserController chatUserController { get; set; }
		private IRepository<ChatUser> chatUserRepository { get; set; }
		private IContext sessionContext { get; set; }

		[WebApiTestInitialise]
		public void SetUp()
		{
			this.sessionContext  = new FakeContext();
			this.chatUserRepository = new ChatUserRepository(this.sessionContext);
			this.chatUserController = new ChatUserController(this.chatUserRepository);
			HttpContext.Current = StaticSessionHttpMock.FakeHttpContext("/test");
		}

		[WebApiTestCleanUp]
		public void CleanUp()
		{
			this.chatUserController = null;
			this.sessionContext = null;
			this.chatUserRepository = null;
			HttpContext.Current = null;
		}

		[WebApiTest]
		public void SaveChatUser()
		{
			ChatUser user = new ChatUser { UserName = "test user", UserToken= "12345" };
			Assert.AreEqual(0, this.sessionContext.ChatUsers.Count(), "the chat user list should be empty on start up");

			IHttpActionResult result = this.chatUserController.Save(user);
			Assert.AreEqual(1, this.sessionContext.ChatUsers.Count(), "the chatuser list was not updated");
			Assert.AreEqual("test user", this.sessionContext.ChatUsers.FirstOrDefault().UserName, "the chatuser name was incorrect");
			Assert.AreEqual("12345", this.sessionContext.ChatUsers.FirstOrDefault().UserToken, "the chatuser token was incorrect");
			Assert.AreEqual(typeof(System.Web.Http.Results.OkResult), result.GetType(), "the result was incorrect");
		}

		[WebApiTest]
		public void SaveChatUserUnique()
		{
			this.sessionContext.ChatUsers = base.GetFakeChatUsers();
			ChatUser user = new ChatUser { UserName = base.GetFakeChatUsers().LastOrDefault().UserName, UserToken = "9876" };
			Assert.AreEqual(base.GetFakeChatUsers().Count(), this.sessionContext.ChatUsers.Count(), "the chat user list should be empty on start up");

			StatusCodeResult result = this.chatUserController.Save(user) as StatusCodeResult;
			Assert.AreEqual(base.GetFakeChatUsers().Count(), this.sessionContext.ChatUsers.Count(), "the chatuser list was updated!");
			Assert.AreEqual("test user", this.sessionContext.ChatUsers.FirstOrDefault().UserName, "the chatuser name was incorrect");
			Assert.AreEqual(this.GetFakeChatUsers().FirstOrDefault().UserToken, this.sessionContext.ChatUsers.FirstOrDefault().UserToken, "the chatuser token was incorrect");
			Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "the result was incorrect");
		}

		[WebApiTest]
		public void GetAllAvailableChatUsers()
		{
			this.sessionContext.ChatUsers = base.GetFakeChatUsers();
			OkNegotiatedContentResult<IEnumerable<ChatUser>> chatUsers = this.chatUserController.GetAllChatUsers() as OkNegotiatedContentResult<IEnumerable<ChatUser>>;

			Assert.AreEqual(this.sessionContext.ChatUsers.Count(), chatUsers.Content.Count(), "the returned chat users were not correct");
		}

		[WebApiTest]
		public void GetChatUserByName()
		{
			this.sessionContext.ChatUsers = base.GetFakeChatUsers();

			OkNegotiatedContentResult<IEnumerable<ChatUser>> chatUsers = this.chatUserController.GetChatUser("test user 1") as OkNegotiatedContentResult<IEnumerable<ChatUser>>;

			Assert.AreEqual(1, chatUsers.Content.Count(), "only one chat user should be returned by the query");
			Assert.AreEqual("test user 1", chatUsers.Content.FirstOrDefault().UserName, "the chat users were not returned properly");
		}

		[WebApiTest]
		public void GetChannelByNameDoesntExist()
		{
			this.sessionContext.ChatUsers = base.GetFakeChatUsers();
			Assert.AreEqual(0, this.sessionContext.ChatUsers.Count(x => x.UserName == "non-existing user"), "make sure test is properly primed");

			OkNegotiatedContentResult<IEnumerable<ChatUser>> chatUsers = this.chatUserController.GetChatUser("non-existing channel") as OkNegotiatedContentResult<IEnumerable<ChatUser>>;
			Assert.AreEqual(0, chatUsers.Content.Count(), "no channels should be returned by the query");
		}

	}
}
