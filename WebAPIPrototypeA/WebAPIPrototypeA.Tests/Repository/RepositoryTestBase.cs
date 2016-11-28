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

		protected List<ChatUser> GetFakeChatUsers(IApplicationSettings fakeApplicationSettings)
		{
			List<ChatUser> fakeChatUsers = new List<ChatUser>();
			fakeChatUsers.Add(new ChatUser { UserName = "test user", UserToken = fakeApplicationSettings.TokenBase });
			fakeChatUsers.Add(new ChatUser { UserName = "test user 1", UserToken = fakeApplicationSettings.TokenBase + 1 });

			return fakeChatUsers;
		}
	}
}
