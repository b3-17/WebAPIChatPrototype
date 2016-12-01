using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web.Http;
using WebAPIPrototypeA.Controllers;
using Moq;
using System.Web;
using Models;
using Repository;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebAPIPrototypeA.Tests
{
	public class ChatMessageControllerTests : ControllerTestBase
	{
		private ChatMessageController chatMessageController { get; set; }
		private IRepository<ChatMessage> chatMessageRepository { get; set; }
		private IContext sessionContext { get; set; }

		[SetUp]
		public void SetUp()
		{
			this.sessionContext = new SessionStateContext();
			this.chatMessageRepository = new ChatMessageRepository(this.sessionContext);
			this.chatMessageController = new ChatMessageController(this.chatMessageRepository);
			HttpContext.Current = StaticHttpMock.FakeHttpContext("/test");
		}

		[TearDown]
		public void CleanUp()
		{
			this.chatMessageController = null;
			this.sessionContext = null;
			this.chatMessageRepository = null;
			HttpContext.Current = null;
		}

		[Test()]
		public void SaveChatMessage()
		{
			ChatMessage channelChatMessage = new ChannelMessage
			{
				Channel = new Channel { ChannelName = "test channel" },
				Message = "test channel message",
				Originator = new ChatUser { UserName = "test user name", UserToken = "12345" }
			};

			Assert.AreEqual(0, this.sessionContext.ChatUsers.Count(), "the chat user list should be empty on start up");

			IHttpActionResult result = this.chatMessageController.Save(base.BuildFakeJsonRequestMessage(channelChatMessage));

			Assert.AreEqual(1, this.sessionContext.ChatMessages.Count(), "the chat message was not saved");
			Assert.AreEqual("test channel", (this.sessionContext.ChatMessages.FirstOrDefault() as ChannelMessage).Channel.ChannelName, "the channel name was incorrect");
			Assert.AreEqual("test channel message", this.sessionContext.ChatMessages.FirstOrDefault().Message, "the message was incorrect");
			Assert.AreEqual("test user name", this.sessionContext.ChatMessages.FirstOrDefault().Originator.UserName, "the user was incorrect");
			Assert.AreEqual("12345", this.sessionContext.ChatMessages.FirstOrDefault().Originator.UserToken, "the user token was incorrect");

			Assert.AreEqual(typeof(System.Web.Http.Results.OkResult), result.GetType(), "the result was incorrect");

			ChatMessage directMessageToSave = new DirectMessage
			{
				To = new ChatUser { UserName = "user to", UserToken = "98765" },
				Message = "Test channel message",
				Originator = new ChatUser { UserName = "test user", UserToken = "12345" }
			};


			result = this.chatMessageController.Save(base.BuildFakeJsonRequestMessage(directMessageToSave));

			Assert.AreEqual(2, this.sessionContext.ChatMessages.Count(), "values were added in error or wiped out!");
			Assert.AreEqual("user to", (this.sessionContext.ChatMessages.LastOrDefault() as DirectMessage).To.UserName, "the user to send to was incorrect");
			Assert.AreEqual("98765", (this.sessionContext.ChatMessages.LastOrDefault() as DirectMessage).To.UserToken, "the user to send to token was incorrect");
			Assert.AreEqual("Test channel message", this.sessionContext.ChatMessages.LastOrDefault().Message, "the message was incorrect");
			Assert.AreEqual("test user", this.sessionContext.ChatMessages.LastOrDefault().Originator.UserName, "the user was incorrect");
			Assert.AreEqual("12345", this.sessionContext.ChatMessages.LastOrDefault().Originator.UserToken, "the user token was incorrect");

			Assert.AreEqual(typeof(System.Web.Http.Results.OkResult), result.GetType(), "the result was incorrect");
		}

		[Test]
		public void SearchChatMessages()
		{ 
			
		}
	}
}