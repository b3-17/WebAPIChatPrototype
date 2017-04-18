using System.Collections.Generic;
using System.Web;
using System.Linq;
using NUnit.Framework;
using Repository;
using Models;

namespace WebAPIPrototypeA.Tests
{
	[WebApiTestClass]
	public class ChannelRepositoryTests : RepositoryTestBase
	{
		private IRepository<Channel> channelRepository { get; set; }
		private IContext fakeContext { get; set; }

		[WebApiTestInitialise]
		public void SetUp()
		{
			fakeContext = new FakeContext();
			this.channelRepository = new ChannelRepository(this.fakeContext);
		}

		[WebApiTestCleanUp]
		public void CleanUp()
		{
			this.channelRepository = null;
			this.fakeContext = null;
		}

		[WebApiTest]
		public void SaveChannel()
		{
			Channel channelToSave = new Channel { ChannelName = "test channel", Subscribers = new List<ChatUser> { new ChatUser { UserName = "test user" } } };
			this.channelRepository.Save(channelToSave);

			List<Channel> channels = this.fakeContext.Channels.ToList();

			Assert.AreEqual(1, channels.Count(), "the channel was not saved correctly");
			Assert.AreEqual(1, channels.FirstOrDefault().Subscribers.Count(), "the subscriber list was not saved correctly");
			Assert.AreEqual("test channel", channels.FirstOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual("test user", channels.FirstOrDefault().Subscribers.FirstOrDefault().UserName, "the user was incorrect");
		}

		[WebApiTest]
		public void SaveMultipleChannelsDontLoseOriginal()
		{
			this.fakeContext.Channels = this.GetFakeChannels();
			Channel channelToSave = new Channel { ChannelName = "test channel 3", Subscribers = new List<ChatUser> { new ChatUser { UserName = "test user 3" } } };
			this.channelRepository.Save(channelToSave);

			List<Channel> channels = this.fakeContext.Channels.ToList();

			Assert.AreEqual(3, channels.Count(), "the channel was not saved correctly - not validating dupes here, so the details are irrelevant");
			Assert.AreEqual(1, channels.FirstOrDefault().Subscribers.Count(), "the subscriber list was not saved correctly");
			Assert.AreEqual("test channel 3", channels.LastOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual("test user 3", channels.LastOrDefault().Subscribers.FirstOrDefault().UserName, "the user was incorrect");
		}

		[WebApiTest]
		public void GetAllChannels()
		{
			this.fakeContext.Channels = this.GetFakeChannels();

			var channels = this.channelRepository.All();

			Assert.AreEqual(this.GetFakeChannels().Count(), channels.Count(), "the channes were not pulled properly");
			Assert.AreEqual(this.GetFakeChannels().FirstOrDefault().Subscribers.Count(), channels.FirstOrDefault().Subscribers.Count(), "the subscriber list was not saved correctly");
			Assert.AreEqual(this.GetFakeChannels().LastOrDefault().ChannelName, channels.LastOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual(this.GetFakeChannels().LastOrDefault().Subscribers.FirstOrDefault().UserName, channels.LastOrDefault().Subscribers.FirstOrDefault().UserName, "the user was incorrect");
		}

		private List<Channel> GetFakeChannels()
		{
			List<Channel> fakeChannels = new List<Channel>();
			fakeChannels.Add(new Channel { ChannelName = "test channel 1", Subscribers = new List<ChatUser> { new ChatUser { UserName = "test user 1" } } });
			fakeChannels.Add(new Channel { ChannelName = "test channel 2", Subscribers = new List<ChatUser> { new ChatUser { UserName = "test user 1" } } });

			return fakeChannels;
		}
	}
}
