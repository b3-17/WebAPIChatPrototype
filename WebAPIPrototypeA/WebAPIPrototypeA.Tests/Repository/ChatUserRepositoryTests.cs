using NUnit.Framework;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using Repository;
using Models;

namespace WebAPIPrototypeA.Tests
{
	[TestFixture()]
	public class ChatUserRepositoryTests : RepositoryTestBase
	{
		private IRepository<ChatUser> userRepository { get; set; }
		private IContext fakeContext { get; set; }

		[SetUp]
		public void SetUp()
		{
			base.SetUpFakeHttpSessionMock("/test");
			fakeContext = new SessionStateContext();
			this.userRepository = new ChatUserRepository(this.fakeContext);
		}

		[TearDown]
		public void CleanUp()
		{
			this.userRepository = null;
		}

		[Test]
		public void SaveUser()
		{
			ChatUser userToSave = new ChatUser { UserName = "test user", UserToken = "12345" };
			this.userRepository.Save(userToSave);

			List<ChatUser> users = HttpContext.Current.Session["ChatUsers"] as List<ChatUser>;

			Assert.AreEqual(1, users.Count(), "the user was not saved correctly");
			Assert.AreEqual("test user", users.FirstOrDefault().UserName, "the user name was incorrect");
			Assert.AreEqual("12345", users.FirstOrDefault().UserToken, "the user token was incorrect");
		}

		[Test()]
		public void GetAllChatUsers()
		{
			HttpContext.Current.Session["ChatUsers"] = this.GetFakeChatUsers();

			var users = this.userRepository.All();

			Assert.AreEqual(this.GetFakeChatUsers().Count(), users.Count(), "the channes were not pulled properly");

			Assert.AreEqual(this.GetFakeChatUsers().LastOrDefault().UserName, users.LastOrDefault().UserName, "the channel name was incorrect");
			Assert.AreEqual(this.GetFakeChatUsers().LastOrDefault().UserToken, users.LastOrDefault().UserToken, "the user was incorrect");
		}

		private List<ChatUser> GetFakeChatUsers()
		{
			List<ChatUser> fakeChatUsers = new List<ChatUser>();
			fakeChatUsers.Add(new ChatUser { UserName = "test user", UserToken = "12345" });
			fakeChatUsers.Add(new ChatUser { UserName = "test user 1", UserToken = "54321" });

			return fakeChatUsers;
		}
	}
}
