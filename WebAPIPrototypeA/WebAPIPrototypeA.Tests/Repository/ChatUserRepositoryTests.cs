using NUnit.Framework;
using System.Web;
using System.Configuration;
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
		private string tokenBaseKey { get { return "TokenBase"; } }

		[SetUp]
		public void SetUp()
		{
			fakeContext = new FakeContext();
			this.userRepository = new ChatUserRepository(this.fakeContext);
		}

		[TearDown]
		public void CleanUp()
		{
			this.userRepository = null;
			this.fakeContext = null;
		}

		[Test]
		public void SaveUser()
		{
			ChatUser userToSave = new ChatUser { UserName = "test user", UserToken = "12345" };
			this.userRepository.Save(userToSave);

			List<ChatUser> users = this.fakeContext.ChatUsers.ToList();

			Assert.AreEqual(1, users.Count(), "the user was not saved correctly");
			Assert.AreEqual("test user", users.FirstOrDefault().UserName, "the user name was incorrect");
			Assert.AreEqual("12345", users.FirstOrDefault().UserToken, "the user token was incorrect");
		}

		[Test()]
		public void GetAllChatUsers()
		{
			this.fakeContext.ChatUsers = base.GetFakeChatUsers();

			var users = this.userRepository.All();

			Assert.AreEqual(base.GetFakeChatUsers().Count(), users.Count(), "the channes were not pulled properly");

			Assert.AreEqual(base.GetFakeChatUsers().LastOrDefault().UserName, users.LastOrDefault().UserName, "the channel name was incorrect");
			Assert.AreEqual(base.GetFakeChatUsers().LastOrDefault().UserToken, users.LastOrDefault().UserToken, "the user was incorrect");
		}
	}
}
