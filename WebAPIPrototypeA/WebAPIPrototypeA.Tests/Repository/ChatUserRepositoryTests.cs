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
		private IApplicationSettings fakeApplicationSettings { get; set; }
		private string tokenBaseKey { get { return "TokenBase"; } }

		[SetUp]
		public void SetUp()
		{
			base.SetUpFakeHttpSessionMock("/test");
			fakeContext = new SessionStateContext();
			this.fakeApplicationSettings = new FakeApplicationSettings();
			this.userRepository = new ChatUserRepository(this.fakeContext, this.fakeApplicationSettings);
		}

		[TearDown]
		public void CleanUp()
		{
			this.userRepository = null;
			this.fakeContext = null;
			this.fakeApplicationSettings = null;
		}

		[Test]
		public void SaveUser()
		{
			ChatUser userToSave = new ChatUser { UserName = "test user" };
			this.userRepository.Save(userToSave);

			List<ChatUser> users = HttpContext.Current.Session["ChatUsers"] as List<ChatUser>;

			Assert.AreEqual(1, users.Count(), "the user was not saved correctly");
			Assert.AreEqual("test user", users.FirstOrDefault().UserName, "the user name was incorrect");
			Assert.AreEqual(this.fakeApplicationSettings.TokenBase, users.FirstOrDefault().UserToken, "the user token was incorrect");
		}

		[Test]
		public void SaveUserIncrementUserToken()
		{
			HttpContext.Current.Session["ChatUsers"] = base.GetFakeChatUsers(this.fakeApplicationSettings);
			ChatUser userToSave = new ChatUser { UserName = "test user 3" };
			this.userRepository.Save(userToSave);

			List<ChatUser> users = HttpContext.Current.Session["ChatUsers"] as List<ChatUser>;

			Assert.AreEqual(3, users.Count(), "the user was not saved correctly");
			Assert.AreEqual("test user 3", users.LastOrDefault().UserName, "the user name was incorrect");
			//Assert.AreEqual(3, users.LastOrDefault().UserToken, "the user token was incorrect");
			Assert.AreEqual(this.fakeApplicationSettings.TokenBase + users.Count() - 1, users.Max(x => x.UserToken), "the user token was incorrect");
		}

		[Test()]
		public void GetAllChatUsers()
		{
			HttpContext.Current.Session["ChatUsers"] = base.GetFakeChatUsers(this.fakeApplicationSettings);

			var users = this.userRepository.All();

			Assert.AreEqual(base.GetFakeChatUsers(this.fakeApplicationSettings).Count(), users.Count(), "the channes were not pulled properly");

			Assert.AreEqual(base.GetFakeChatUsers(this.fakeApplicationSettings).LastOrDefault().UserName, users.LastOrDefault().UserName, "the channel name was incorrect");
			Assert.AreEqual(base.GetFakeChatUsers(this.fakeApplicationSettings).LastOrDefault().UserToken, users.LastOrDefault().UserToken, "the user was incorrect");
		}


	}
}
