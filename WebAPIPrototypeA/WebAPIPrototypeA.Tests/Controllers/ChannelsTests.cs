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
using System.Reflection;

namespace WebAPIPrototypeA.Tests
{
	[WebApiTestClass]
	public class ChannelsTests : ControllerTestBase
	{
		private ChannelsController channelController { get; set; }
		private IRepository<Channel> channelRepository { get; set; }
		private IContext sessionContext { get; set; }

		public void RunTests()
		{
			
		}

		[WebApiTestInitialise]
		public void SetUp()
		{
			this.sessionContext  = new SessionStateContext();
			this.channelRepository = new ChannelRepository(this.sessionContext);
			this.channelController = new ChannelsController(this.channelRepository);
			HttpContext.Current = StaticHttpMock.FakeHttpContext("/test");
		}

		[WebApiTestCleanUp]
		public void CleanUp()
		{
			this.channelController = null;
		}

		[WebApiTest]
		public void SaveChannel()
		{
			Channel channel = new Channel { ChannelName = "test channel", Subscribers = new List<ChatUser> { new ChatUser { UserName = "new user" } } };
			Assert.AreEqual(0, this.sessionContext.Channels.Count(), "the channel list should be empty on start up");

			IHttpActionResult result = this.channelController.Save(channel);
			Assert.AreEqual(1, this.sessionContext.Channels.Count(), "the chat channel list was not updated");
			Assert.AreEqual("new user", this.sessionContext.Channels.FirstOrDefault().Subscribers.FirstOrDefault().UserName, "a new channel should be auto subscribed by the initial user");
			Assert.AreEqual("test channel", this.sessionContext.Channels.FirstOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual(typeof(System.Web.Http.Results.OkResult), result.GetType(), "the result was incorrect");
		}

		[WebApiTest]
		public void SaveChannelNoInitialSubscriber()
		{
			Channel channel = new Channel { ChannelName = "test channel", Subscribers = null };
			Assert.AreEqual(0, sessionContext.Channels.Count(), "the channel list should be empty on start up");

			StatusCodeResult result = this.channelController.Save(channel) as StatusCodeResult;
			Assert.AreEqual(0, this.sessionContext.Channels.Count(), "the chat channel list was updated");
			Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "a new channel should not be created with null subscribers");

			channel = new Channel { ChannelName = "test channel", Subscribers = new List<ChatUser>() };

			result = this.channelController.Save(channel) as StatusCodeResult;
			Assert.AreEqual(0, this.sessionContext.Channels.Count(), "the chat channel list was updated");
			Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "the result status code was incorrect");
		}

		[WebApiTest]
		public void CreateChannelAlreadyExists()
		{
			Channel existingChannel = new Channel { ChannelName = "existing test channel" };
			this.sessionContext.Channels = new List<Channel> { existingChannel };
			Assert.AreEqual(1, this.sessionContext.Channels.Count(), "the channel list should be primed to test dupes");

			StatusCodeResult result = this.channelController.Save(existingChannel) as StatusCodeResult;
			Assert.AreEqual(1, this.sessionContext.Channels.Count(), "the chat channel list was not updated");
			Assert.AreEqual("existing test channel", this.sessionContext.Channels.FirstOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "the result was incorrect");
		}

		[WebApiTest]
		public void GetAllAvailableChannels()
		{
			this.sessionContext.Channels = this.GetFakeChannels();
			OkNegotiatedContentResult<IEnumerable<Channel>> channels = this.channelController.GetAllChannels() as OkNegotiatedContentResult<IEnumerable<Channel>>;

			Assert.AreEqual(this.sessionContext.Channels.Count(), channels.Content.Count(), "the returned channels were not correct");
		}

		[WebApiTest]
		public void GetChannelByName()
		{
			this.sessionContext.Channels = this.GetFakeChannels();
			this.sessionContext.Channels.ToList().Add(new Channel { ChannelName = "test channel 3" });
			OkNegotiatedContentResult<IEnumerable<Channel>> channels = this.channelController.GetChannel("test channel 2") as OkNegotiatedContentResult<IEnumerable<Channel>>;

			Assert.AreEqual(1, channels.Content.Count(), "only one channel should be returned by the query");
			Assert.AreEqual("test channel 2", channels.Content.FirstOrDefault().ChannelName, "the channels were not returned properly");
		}

		[WebApiTest]
		public void GetChannelByNameDoesntExist()
		{
			this.sessionContext.Channels = this.GetFakeChannels();
			Assert.AreEqual(0, this.sessionContext.Channels.Count(x => x.ChannelName == "non-existing channel"), "make sure test is properly primed");

			OkNegotiatedContentResult<IEnumerable<Channel>> channels = this.channelController.GetChannel("non-existing channel") as OkNegotiatedContentResult<IEnumerable<Channel>>;
			Assert.AreEqual(0, channels.Content.Count(), "no channels should be returned by the query");
		}

		[WebApiTest]
		public void SubscribeUserToExistentChannel()
		{
			this.sessionContext.Channels = this.GetFakeChannels();
			Channel subscribeToChannel = new Channel { ChannelName = "test channel 2", Subscribers = new List<ChatUser> { new ChatUser { UserName = "test user 1" } } };

			OkResult result = this.channelController.Subscribe(subscribeToChannel) as OkResult;
			Assert.AreEqual(1, this.sessionContext.Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the channel was not properly subscribed to");
			Assert.AreEqual("test user 1", this.sessionContext.Channels.FirstOrDefault(x => x.ChannelName == "test channel 2")
							.Subscribers.FirstOrDefault().UserName, "the user name was incorrect");
			Assert.IsNotNull(result, "the response status was not correct");
		}

		[WebApiTest]
		public void SubscribeUserToExistentChannelAlreadySubscribed()
		{
			this.sessionContext.Channels = this.GetFakeChannels();
			this.sessionContext.Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers = new List<ChatUser>();
			this.sessionContext.Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Add(new ChatUser { UserName = "test user 1" });
			ConflictResult result = this.channelController.Subscribe(
				new Channel { ChannelName = "test channel 2", Subscribers = new List<ChatUser> { new ChatUser { UserName = "test user 1" } } }) as ConflictResult;
			Assert.AreEqual(1, this.sessionContext.Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the channel was not properly subscribed to");
			Assert.IsNotNull(result, "the response status was not correct");
		}

		[WebApiTest]
		public void SubscribeUserToNonExistentChannel()
		{
			Assert.AreEqual(0, this.sessionContext.Channels.Count(), "make sure we have no channels currently");

			OkResult result = this.channelController.Subscribe(new Channel { ChannelName = "test channel 2",
				Subscribers = new List<ChatUser> { new ChatUser { UserName = "test user 1" } } }) as OkResult;
			Assert.AreEqual(1, this.sessionContext.Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the channel was not created by default");
			Assert.IsNotNull(result, "the response status was not correct");
		}

		[WebApiTest]
		public void UnsubscribeUserFromExistentChannel()
		{
			this.sessionContext.Channels = this.GetFakeChannels();
			this.sessionContext.Channels.ToList().ForEach(x => x.Subscribers = new List<ChatUser>());

			this.sessionContext.Channels.FirstOrDefault(x => x.ChannelName == "test channel 1").Subscribers.Add(new ChatUser { UserName = "test user 1" });
			this.sessionContext.Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Add(new ChatUser { UserName = "test user 1" });

			OkResult result = this.channelController.Unsubscribe(
				new Channel { ChannelName = "test channel 1", Subscribers = new List<ChatUser> { new ChatUser { UserName = "test user 1" } } }) as OkResult;
			Assert.AreEqual(0, this.sessionContext.Channels.FirstOrDefault(x => x.ChannelName == "test channel 1").Subscribers.Count(), "the user was not unsubscribed");
			Assert.AreEqual(1, this.sessionContext.Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the user was unsubscribed from the wrong channel!");

			Assert.IsNotNull(result, "the response status was not correct");
		}

		[WebApiTest]
		public void UnsubscribeUserFromNonExistentChannel()
		{
			Assert.AreEqual(0, this.sessionContext.Channels.Count(), "make sure we have no channels currently");

			OkResult result = this.channelController.Unsubscribe(
				new Channel { ChannelName = "test channel 1", Subscribers = new List<ChatUser> { new ChatUser { UserName = "test user 1" } } }) as OkResult;
			Assert.AreEqual(0, this.sessionContext.Channels.Count(), "make sure we don't create any new channels");
			Assert.IsNotNull(result, "the response status was not correct");
		}

		[WebApiTest]
		public void UnsubscribeUserNotSubscribedFromExistentChannel()
		{
			this.sessionContext.Channels = this.GetFakeChannels();
			this.sessionContext.Channels.ToList().ForEach(x => x.Subscribers = new List<ChatUser>());

			OkResult result = this.channelController.Unsubscribe(
				new Channel { ChannelName = "test channel 1", Subscribers = new List<ChatUser> { new ChatUser { UserName = "test user 1" } } }) as OkResult;
			Assert.AreEqual(2, this.sessionContext.Channels.Count(), "make sure we don't create any new channels");
			Assert.IsNotNull(result, "the response status was not correct");
		}


	}
}
