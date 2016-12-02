using System.Collections.Generic;
using System.Web;
using System.Linq;
using NUnit.Framework;
using Repository;
using Models;

namespace WebAPIPrototypeA.Tests
{
	[TestFixture()]
	public class ChatMessageRepositoryTests : RepositoryTestBase
	{
		private IRepository<ChatMessage> chatMessageRepository { get; set; }
		private IContext fakeContext { get; set; }

		[SetUp]
		public void SetUp()
		{
			fakeContext = new FakeContext();
			this.chatMessageRepository = new ChatMessageRepository(this.fakeContext);
		}

		[TearDown]
		public void CleanUp()
		{
			this.chatMessageRepository = null;
			this.fakeContext = null;
		}

		[Test()]
		public void SaveChannelChatMessage()
		{
			ChatMessage chatMessageToSave = new ChannelMessage
			{
				Channel = new Channel { ChannelName = "test channel" },
				Message = "Test channel message",
				Originator = new ChatUser { UserName = "test user", UserToken = "12345" } };
			this.chatMessageRepository.Save(chatMessageToSave);

			List<ChatMessage> channelMessages = fakeContext.ChatMessages.ToList();

			Assert.AreEqual(1, channelMessages.Count(), "the channel was not saved correctly");
			Assert.AreEqual("test channel", (this.fakeContext.ChatMessages.FirstOrDefault() as ChannelMessage).Channel.ChannelName, "the channel name was incorrect");
			Assert.AreEqual("Test channel message", channelMessages.FirstOrDefault().Message, "the message was incorrect");
			Assert.AreEqual("test user", channelMessages.FirstOrDefault().Originator.UserName, "the user was incorrect");
			Assert.AreEqual("12345", channelMessages.FirstOrDefault().Originator.UserToken, "the user token was incorrect");
			Assert.IsNotNull(channelMessages.FirstOrDefault().ServerTimeDateSent, "the server time stamp was not set");
		}

		[Test()]
		public void SaveDirectChatMessage()
		{
			ChatMessage chatMessageToSave = new DirectMessage {
				To = new ChatUser{ UserName = "user to", UserToken = "98765" },
				Message = "Test channel message",
				Originator = new ChatUser { UserName = "test user", UserToken = "12345" }
			};
			this.chatMessageRepository.Save(chatMessageToSave);

			List<ChatMessage> directMessages = this.fakeContext.ChatMessages.ToList();

			Assert.AreEqual(1, directMessages.Count(), "the channel was not saved correctly");
			Assert.AreEqual("user to", (this.fakeContext.ChatMessages.FirstOrDefault() as DirectMessage).To.UserName, "the user to send to was incorrect");
			Assert.AreEqual("98765", (this.fakeContext.ChatMessages.FirstOrDefault() as DirectMessage).To.UserToken, "the user to send to token was incorrect");
			Assert.AreEqual("Test channel message", directMessages.FirstOrDefault().Message, "the message was incorrect");
			Assert.AreEqual("test user", directMessages.FirstOrDefault().Originator.UserName, "the user was incorrect");
			Assert.AreEqual("12345", directMessages.FirstOrDefault().Originator.UserToken, "the user token was incorrect");
			Assert.IsNotNull(directMessages.FirstOrDefault().ServerTimeDateSent, "the server time stamp was not set");
		}

		[Test]
		public void SaveDirectAndChannelChatMessages()
		{
			ChatMessage channelMessageToSave = new ChannelMessage
			{
				Channel = new Channel { ChannelName = "test channel" },
				Message = "Test channel message",
				Originator = new ChatUser { UserName = "test user", UserToken = "12345" }
			};
			ChatMessage directMessageToSave = new DirectMessage
			{
				To = new ChatUser { UserName = "user to", UserToken = "98765" },
				Message = "Test channel message",
				Originator = new ChatUser { UserName = "test user", UserToken = "12345" }
			};

			this.chatMessageRepository.Save(channelMessageToSave);
			this.chatMessageRepository.Save(directMessageToSave);

			List<ChatMessage> allMessages = this.fakeContext.ChatMessages.ToList();

			Assert.AreEqual(2, allMessages.Count(), "the channel was not saved correctly");

			Assert.AreEqual("test channel", (this.fakeContext.ChatMessages.FirstOrDefault() as ChannelMessage).Channel.ChannelName, "the channel name was incorrect");
			Assert.AreEqual("Test channel message", allMessages.FirstOrDefault().Message, "the message was incorrect");
			Assert.AreEqual("test user", allMessages.FirstOrDefault().Originator.UserName, "the user was incorrect");
			Assert.AreEqual("12345", allMessages.FirstOrDefault().Originator.UserToken, "the user token was incorrect");

			Assert.AreEqual("user to", (this.fakeContext.ChatMessages.LastOrDefault() as DirectMessage).To.UserName, "the user to send to was incorrect");
			Assert.AreEqual("98765", (this.fakeContext.ChatMessages.LastOrDefault() as DirectMessage).To.UserToken, "the user to send to token was incorrect");
			Assert.AreEqual("Test channel message", allMessages.LastOrDefault().Message, "the message was incorrect");
			Assert.AreEqual("test user", allMessages.LastOrDefault().Originator.UserName, "the user was incorrect");
			Assert.AreEqual("12345", allMessages.LastOrDefault().Originator.UserToken, "the user token was incorrect");
		}

		[Test()]
		public void GetAllChannels()
		{
			this.fakeContext.ChatMessages = this.GetFakeChatMessages();

			var chatMessages = this.chatMessageRepository.All();

			Assert.AreEqual(this.GetFakeChatMessages().Count(), chatMessages.Count(), "the channes were not pulled properly");
			Assert.AreEqual(this.GetFakeChatMessages().FirstOrDefault().Message, chatMessages.FirstOrDefault().Message, "the subscriber list was not saved correctly");
			Assert.AreEqual(this.GetFakeChatMessages().LastOrDefault().Originator.UserName, chatMessages.LastOrDefault().Originator.UserName, "the user name was incorrect");
			Assert.AreEqual(this.GetFakeChatMessages().LastOrDefault().Originator.UserToken, chatMessages.LastOrDefault().Originator.UserToken, "the user token was incorrect");
		}

		private List<ChatMessage> GetFakeChatMessages()
		{
			List<ChatMessage> fakeChatMessages = new List<ChatMessage>();
			fakeChatMessages.Add(new ChannelMessage
			{
				Channel = new Channel { ChannelName = "test channel 1" },
				Message = "channel message 1",
				Originator = new ChatUser { UserName = "from user 1", UserToken = "12345" } });
			
			fakeChatMessages.Add(new DirectMessage
			{
				Originator = new ChatUser { UserName = "from user 1", UserToken = "12345" },
				Message = "direct message 1",
				To = new ChatUser { UserName = "to user 2", UserToken = "09876" }});
			
			fakeChatMessages.Add(new DirectMessage
			{
				Originator = new ChatUser { UserName = "from user 2", UserToken = "09876" },
				Message = "direct message 2",
				To = new ChatUser { UserName = "to user 1", UserToken = "12345" }
			});

			return fakeChatMessages;
		}
	}
}
