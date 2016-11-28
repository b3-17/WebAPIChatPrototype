using NUnit.Framework;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using Repository;
using Models;

namespace WebAPIPrototypeA.Tests
{
	[TestFixture()]
	public class UserRepositoryTests : RepositoryTestBase
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
		public void SaveUniqueUser()
		{ 
		ChatUser userToSave = new ChatUser { UserName = "test user", UserToken = "12345" };
			this.userRepository.Save(userToSave);

			List<ChatUser> users = HttpContext.Current.Session["Users"] as List<ChatUser>;

			Assert.AreEqual(1, users.Count(), "the channel was not saved correctly");
			Assert.AreEqual("test user", users.FirstOrDefault().UserName, "the channel name was incorrect");
			Assert.AreEqual("12345", users.FirstOrDefault().UserToken, "the user was incorrect");
		}
	}
}
