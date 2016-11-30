using System.Web;
using System.Configuration;
using Models;
using Repository;
using System.Collections.Generic;
using Moq;

namespace WebAPIPrototypeA.Tests
{
	public class RepositoryTestBase
	{
		protected void SetUpFakeHttpSessionMock(string url)
		{
			HttpContext.Current = StaticHttpMock.FakeHttpContext(url);
		}

		protected List<ChatUser> GetFakeChatUsers()
		{
			List<ChatUser> fakeChatUsers = new List<ChatUser>();
			fakeChatUsers.Add(new ChatUser { UserName = "test user", UserToken = "12345" });
			fakeChatUsers.Add(new ChatUser { UserName = "test user 1", UserToken = "54321" });

			return fakeChatUsers;
		}
	}
}
