using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebAPIPrototypeA.Controllers;
using System.Web.Http.Results;

namespace WebAPIPrototypeA.Tests
{
	[TestFixture()]
	public class ChannelsTests
	{
		private ChannelsController channelController { get; set; }

		[SetUp]
		public void SetUp()
		{
			this.channelController = new ChannelsController();
		}

		[TearDown]
		public void CleanUp()
		{
			this.channelController = null;
		}

		[Test()]
		public void SaveChannel()
		{
			Channel channel = new Channel { ChannelName = "test channel", Subscribers = new List<ChatUser> { new ChatUser { UserName = "new user" } } };
			Assert.AreEqual(0, channelController.Channels.Count(), "the channel list should be empty on start up");

			IHttpActionResult result = this.channelController.Save(channel);
			Assert.AreEqual(1, this.channelController.Channels.Count(), "the chat channel list was not updated");
			Assert.AreEqual("new user", this.channelController.Channels.FirstOrDefault().Subscribers.FirstOrDefault().UserName, "a new channel should be auto subscribed by the initial user");
			Assert.AreEqual("test channel", this.channelController.Channels.FirstOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual(typeof(System.Web.Http.Results.OkResult), result.GetType(), "the result was incorrect");
		}

		[Test()]
		public void SaveChannelNoInitialSubscriber()
		{
			Channel channel = new Channel { ChannelName = "test channel", Subscribers = null };
			Assert.AreEqual(0, channelController.Channels.Count(), "the channel list should be empty on start up");

			StatusCodeResult result = this.channelController.Save(channel) as StatusCodeResult;
			Assert.AreEqual(0, this.channelController.Channels.Count(), "the chat channel list was updated");
			Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "a new channel should not be created with null subscribers");

			channel = new Channel { ChannelName = "test channel", Subscribers = new List<ChatUser>() };

			result = this.channelController.Save(channel) as StatusCodeResult;
			Assert.AreEqual(0, this.channelController.Channels.Count(), "the chat channel list was updated");
			Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "the result status code was incorrect");
		}

		[Test()]
		public void CreateChannelAlreadyExists()
		{
			Channel existingChannel = new Channel { ChannelName = "existing test channel" };
			this.channelController.Channels.Add(existingChannel);
			Assert.AreEqual(1, this.channelController.Channels.Count(), "the channel list should be primed to test dupes");

			StatusCodeResult result = this.channelController.Save(existingChannel) as StatusCodeResult;
			Assert.AreEqual(1, this.channelController.Channels.Count(), "the chat channel list was not updated");
			Assert.AreEqual("existing test channel", this.channelController.Channels.FirstOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "the result was incorrect");
		}

		[Test()]
		public void GetAllAvailableChannels()
		{
			this.channelController.Channels = this.GetFakeChannels();
			OkNegotiatedContentResult<List<Channel>> channels = this.channelController.GetAllChannels() as OkNegotiatedContentResult<List<Channel>>;

			Assert.AreEqual(this.channelController.Channels.Count(), channels.Content.Count(), "the returned channels were not correct");
		}

		[Test()]
		public void GetChannelByName()
		{
			this.channelController.Channels = this.GetFakeChannels();
			this.channelController.Channels.Add(new Channel { ChannelName = "test channel 3" });
			OkNegotiatedContentResult<IEnumerable<Channel>> channels = this.channelController.GetChannel("test channel 2") as OkNegotiatedContentResult<IEnumerable<Channel>>;

			Assert.AreEqual(1, channels.Content.Count(), "only one channel should be returned by the query");
			Assert.AreEqual("test channel 2", channels.Content.FirstOrDefault().ChannelName, "the channels were not returned properly");
		}

		[Test()]
		public void GetChannelByNameDoesntExist()
		{
			this.channelController.Channels = this.GetFakeChannels();
			Assert.AreEqual(0, this.channelController.Channels.Count(x => x.ChannelName == "non-existing channel"), "make sure test is properly primed");

			OkNegotiatedContentResult<IEnumerable<Channel>> channels = this.channelController.GetChannel("non-existing channel") as OkNegotiatedContentResult<IEnumerable<Channel>>;
			Assert.AreEqual(0, channels.Content.Count(), "no channels should be returned by the query");
		}

		/*[Test()]
		public void SubscribeUserToExistentChannel()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			this.SetUpFakeChannels(service);
			service.SubscribeUserToChannel(new Channel { ChannelName = "test channel 2" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(1, (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the channel was not properly subscribed to");
			Assert.AreEqual("test user 1", (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2")
							.Subscribers.FirstOrDefault().UserName, "the user name was incorrect");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void SubscribeUserToExistentChannelAlreadySubscribed()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			this.SetUpFakeChannels(service);
			(service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Add(new ChatUser { UserName = "test user 1" });
			service.SubscribeUserToChannel(new Channel { ChannelName = "test channel 2" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(1, (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the channel was not properly subscribed to");
			Assert.AreEqual(System.Net.HttpStatusCode.Conflict, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void SubscribeUserToNonExistentChannel()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			Assert.AreEqual(0, (service as ChatService).Channels.Count(), "make sure we have no channels currently");

			service.SubscribeUserToChannel(new Channel { ChannelName = "test channel 2" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(1, (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the channel was not created by default");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void UnsubscribeUserFromExistentChannel()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			this.SetUpFakeChannels(service);
			(service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 1").Subscribers.Add(new ChatUser { UserName = "test user 1" });
			(service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Add(new ChatUser { UserName = "test user 1" });

			service.UnsubscribeUserFromChannel(new Channel { ChannelName = "test channel 1" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(0, (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 1").Subscribers.Count(), "the user was not unsubscribed");
			Assert.AreEqual(1, (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the user was unsubscribed from the wrong channel!");

			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void UnsubscribeUserFromNonExistentChannel()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			Assert.AreEqual(0, (service as ChatService).Channels.Count(), "make sure we have no channels currently");

			service.UnsubscribeUserFromChannel(new Channel { ChannelName = "test channel 1" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(0, (service as ChatService).Channels.Count(), "make sure we don't create any new channels");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void UnsubscribeUserNotSubscribedFromExistentChannel()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			this.SetUpFakeChannels(service);

			service.UnsubscribeUserFromChannel(new Channel { ChannelName = "test channel 1" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(2, (service as ChatService).Channels.Count(), "make sure we don't create any new channels");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}*/

		private List<Channel> GetFakeChannels()
		{
			List<Channel> fakeChannels = new List<Channel>();
			fakeChannels.Add(new Channel { ChannelName = "test channel 1" });
			fakeChannels.Add(new Channel { ChannelName = "test channel 2" });

			return fakeChannels;
		}
	}
}
