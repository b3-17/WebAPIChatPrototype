using System.Web;
using System.Collections.Generic;
using Models;
using Repository;

namespace WebAPIPrototypeA.Tests
{
	public class ControllerTestBase
	{
		protected List<Channel> GetFakeChannels()
		{
			List<Channel> fakeChannels = new List<Channel>();
			fakeChannels.Add(new Channel { ChannelName = "test channel 1" });
			fakeChannels.Add(new Channel { ChannelName = "test channel 2" });

			return fakeChannels;
		}

		protected List<ChatUser> GetFakeChatUsers()
		{
			List<ChatUser> fakeChatUsers = new List<ChatUser>();
			fakeChatUsers.Add(new ChatUser { UserName = "test user", UserToken = "12345" });
			fakeChatUsers.Add(new ChatUser { UserName = "test user 1", UserToken = "54321" });

			return fakeChatUsers;
		}

		protected void SetUpFakeHttpSessionMock(string url)
		{
			HttpContext.Current = StaticHttpMock.FakeHttpContext(url);
		}
	}
}
