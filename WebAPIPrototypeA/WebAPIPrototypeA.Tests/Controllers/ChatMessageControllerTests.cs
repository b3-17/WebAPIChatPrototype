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
using System.Web.Http.Results;

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
			this.sessionContext = new FakeContext();
			this.chatMessageRepository = new ChatMessageRepository(this.sessionContext);
			this.chatMessageController = new ChatMessageController(this.chatMessageRepository);
			HttpContext.Current = StaticSessionHttpMock.FakeHttpContext("/test");
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
		public void SearchDirectChatMessages()
		{
			this.sessionContext.ChatMessages = this.GetFakeChatMessages();
			int directMessagesCount = this.GetFakeChatMessages().OfType<DirectMessage>().Count(x => x.Originator.UserToken == "09876" || x.To.UserToken == "09876");
			OkNegotiatedContentResult<List<ChatMessage>> results = this.chatMessageController.GetChatMessages("09876", string.Empty) as OkNegotiatedContentResult<List<ChatMessage>>;

			Assert.AreEqual(directMessagesCount, results.Content.OfType<DirectMessage>().Count(), "the wrong results were returned");
			Assert.AreEqual(results.Content.OfType<ChannelMessage>().Count(), 0, "the wrong results were returned");
		}

		[Test]
		public void SearchChannelChatMessages()
		{
			List<ChatMessage> fakeMessages = this.GetFakeChatMessages();
			fakeMessages.Add(new ChannelMessage
			{
				Channel = new Channel
				{ ChannelName = "test channel 2" },
				Message = "test message",
				Originator = new ChatUser { UserName = "test user", UserToken = "42938" } });
			
			this.sessionContext.ChatMessages = fakeMessages;
			OkNegotiatedContentResult<List<ChatMessage>> results = this.chatMessageController.GetChatMessages(string.Empty, "test channel 2") as OkNegotiatedContentResult<List<ChatMessage>>;

			Assert.AreEqual(fakeMessages.OfType<ChannelMessage>().Count(x => x.Channel.ChannelName == "test channel 2"), 
			                results.Content.OfType<ChannelMessage>().Count(), "the wrong results were returned");
			Assert.AreEqual(results.Content.OfType<DirectMessage>().Count(), 0, "the wrong results were returned");
		}
	}
}