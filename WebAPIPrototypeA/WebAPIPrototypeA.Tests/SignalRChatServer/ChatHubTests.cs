using NUnit.Framework;
using Microsoft.AspNet.SignalR;
using Moq;
using SignalRChatServer;
using Microsoft.AspNet.SignalR.Hubs;
using System.Dynamic;
using System;

namespace WebAPIPrototypeA.Tests
{
	[WebApiTestClass]
	public class ChatHubTests : SignalRServerTestBase
	{
		private Mock<IConnection> mockConnection { get; set; }
		private Mock<IGroupManager> mockGroupManager { get; set; }
		private bool isSent { get; set; }

		[WebApiTestInitialise]
		public void SetUp()
		{
			this.mockConnection = new Mock<IConnection>();
			this.mockGroupManager = new Mock<IGroupManager>();
			this.isSent = false;
		}

		[WebApiTestCleanUp]
		public void CleanUp()
		{
			base.ClearTestRequest();
			this.mockConnection = null;
			this.mockGroupManager = null;
		}

		[WebApiTest]
		public void CreateChatRoom()
		{
			string testChannelName = "testChannel";
			string testUserName = "testUser";
			string contextId = "1";
			dynamic channel = new ExpandoObject();

			IRequest request = base.BuildTestRequest().Object;
			var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
			mockClients.Setup(m => m.All).Returns((ExpandoObject)channel);
			mockGroupManager.Setup(m => m.Add(contextId, testChannelName)).Callback(() => this.isSent = true );

			var mockHubCallerContext = new Mock<HubCallerContext>(request, contextId);

			channel.sendChannelCreationMessage = new Action<string, string>((channelName, userName) =>
			{
				this.isSent = true;
				Assert.AreEqual(testChannelName, channelName, "the channel was not supplied correctly");
				Assert.AreEqual(testUserName, userName, "the user name was not supplied correctly");
			});

			ChatHub hub = new ChatHub()
			{
				Context = mockHubCallerContext.Object,
				Clients = mockClients.Object,
				Groups = mockGroupManager.Object
			};

			hub.CreateChatChannel(testChannelName, testUserName);
		}

		[WebApiTest]
		public void BroadcastMessageToChannel()
		{
			string testChannel = "testChannel";
			string testMessage = "test message";
			string testUser = "testUser";
			dynamic broadCast = new ExpandoObject();

			broadCast.sendChannelMessage = new Action<string, string>((user, message) =>
		   {
			   this.isSent = true;
			   Assert.AreEqual(testUser, user, "the user was not supplied correctly");
			   Assert.AreEqual(testMessage, message, "the message was not supplied correctly");
			});

			IRequest request = base.BuildTestRequest().Object;
			var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
			mockClients.Setup(m => m.Group(testChannel)).Returns((ExpandoObject)broadCast);

			var mockHubCallerContext = new Mock<HubCallerContext>(request, "1");

			ChatHub hub = new ChatHub()
			{
				Context = mockHubCallerContext.Object,
				Clients = mockClients.Object,
				Groups = mockGroupManager.Object
			};

			hub.ChannelBroadCast(testChannel, testUser, testMessage);
		}

		[WebApiTest]
		public void DirectMessageUser()
		{
			string testMessage = "testMessage";
			string testFrom = "test from";
			string testUserId = "12345";
			dynamic directMessage = new ExpandoObject();

			directMessage.sendDirectMessage = new Action<string, string>((userName, message) =>
		   {
			   this.isSent = true;
			   Assert.AreEqual(testMessage, message, "the message was not supplied correctly");
			   Assert.AreEqual(testFrom, userName, "the user name was not supplied correctly");
		   });

			IRequest request = base.BuildTestRequest().Object;
			var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
			mockClients.Setup(m => m.Client(testUserId)).Returns((ExpandoObject)directMessage);

			var mockHubCallerContext = new Mock<HubCallerContext>(request, "1");

			ChatHub hub = new ChatHub()
			{
				Context = mockHubCallerContext.Object,
				Clients = mockClients.Object,
				Groups = mockGroupManager.Object
			};

			hub.DirectMessageUser(testUserId, testFrom, testMessage);
		}

		[WebApiTest]
		public void CreateUser()
		{
			string testUser = "test from";
			string testMessage = String.Format("A user has signed up: {0}", testUser);
			dynamic userSignup = new ExpandoObject();

			userSignup.sendUserCreatedMessage = new Action<string>((user) =>
		   {
			   this.isSent = true;
			   Assert.AreEqual(testMessage, user, "the user was not supplied correctly");
		   });

			IRequest request = base.BuildTestRequest().Object;
			var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
			mockClients.Setup(m => m.All).Returns((ExpandoObject)userSignup);

			var mockHubCallerContext = new Mock<HubCallerContext>(request, "1");

			ChatHub hub = new ChatHub()
			{
				Context = mockHubCallerContext.Object,
				Clients = mockClients.Object,
				Groups = mockGroupManager.Object
			};

			hub.CreateUser(testUser);
		}

		[WebApiTest]
		public void SubscribeUser()
		{
			string contextId = "1";
			string testChannel = "testChannel";
			string testUser = "test from";
			string testMessage = String.Format("{0} joined {1}", testUser, testChannel);
			dynamic sub = new ExpandoObject();

			sub.sendChannelJoiningMessage = new Action<string, string>((user, message) =>
		   {
			   this.isSent = true;
			   Assert.AreEqual(testUser, user, "the user was not supplied correctly");
			   Assert.AreEqual(testMessage, message, "the user was not supplied correctly");
		   });

			IRequest request = base.BuildTestRequest().Object;
			var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
			mockClients.Setup(m => m.Group(testChannel)).Returns((ExpandoObject)sub);
			mockGroupManager.Setup(m => m.Add(contextId, testChannel)).Callback(() => this.isSent = true);

			var mockHubCallerContext = new Mock<HubCallerContext>(request, "1");

			ChatHub hub = new ChatHub()
			{
				Context = mockHubCallerContext.Object,
				Clients = mockClients.Object,
				Groups = mockGroupManager.Object
			};

			hub.JoinChatChannel(testChannel, contextId, testUser);
		}

		[WebApiTest]
		public void UnsubscribeUser()
		{
			string contextId = "1";
			string testChannel = "testChannel";
			string testUser = "test from";
			string testMessage = String.Format("{0} left {1}", testUser, testChannel);
			dynamic unsub = new ExpandoObject();

			unsub.sendChannelLeavingMessage = new Action<string, string>((user, message) =>
		   {
			   this.isSent = true;
			   Assert.AreEqual(testUser, user, "the user was not supplied correctly");
				Assert.AreEqual(testMessage, message, "the user was not supplied correctly");
		   });

			IRequest request = base.BuildTestRequest().Object;
			var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
			mockClients.Setup(m => m.Group(testChannel)).Returns((ExpandoObject)unsub);
			mockGroupManager.Setup(m => m.Remove(contextId, testChannel)).Callback(() => this.isSent = true);

			var mockHubCallerContext = new Mock<HubCallerContext>(request, "1");

			ChatHub hub = new ChatHub()
			{
				Context = mockHubCallerContext.Object,
				Clients = mockClients.Object,
				Groups = mockGroupManager.Object
			};

			hub.LeaveChatChannel(testChannel, contextId, testUser);
		}
	}
}
