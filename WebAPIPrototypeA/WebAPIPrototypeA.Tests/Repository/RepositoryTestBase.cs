using System.Web;
using Models;
using System.Collections.Generic;

namespace WebAPIPrototypeA.Tests
{
	public class RepositoryTestBase
	{
		protected void SetUpFakeHttpSessionMock(string url)
		{
			StaticCacheHttpMock.SetBasicHttpMock();
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
