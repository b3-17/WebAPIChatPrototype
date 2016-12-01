using System.Web;
using System.Collections.Generic;
using Models;
using Newtonsoft.Json;
using System.Net.Http;

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

		protected List<ChatMessage> GetFakeChatMessages()
		{
			List<ChatMessage> fakeChatMessages = new List<ChatMessage>();
			fakeChatMessages.Add(new ChannelMessage
			{
				Channel = new Channel { ChannelName = "test channel 1" },
				Message = "channel message 1",
				Originator = new ChatUser { UserName = "from user 1", UserToken = "12345" }
			});

			fakeChatMessages.Add(new DirectMessage
			{
				Originator = new ChatUser { UserName = "from user 1", UserToken = "12345" },
				Message = "direct message 1",
				To = new ChatUser { UserName = "to user 2", UserToken = "09876" }
			});

			fakeChatMessages.Add(new DirectMessage
			{
				Originator = new ChatUser { UserName = "from user 2", UserToken = "09876" },
				Message = "direct message 2",
				To = new ChatUser { UserName = "to user 1", UserToken = "12345" }
			});

			return fakeChatMessages;
		}

		protected HttpRequestMessage BuildFakeJsonRequestMessage(object toJsonSerialise)
		{ 
			string jsonMessage = JsonConvert.SerializeObject(toJsonSerialise);
			var request = new HttpRequestMessage();
			var content = new StringContent(jsonMessage);

			request.Content = content;

			return request;
		}
	}
}
