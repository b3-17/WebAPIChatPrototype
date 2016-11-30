using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using Repository;
using Models;

namespace WebAPIPrototypeA.Controllers
{
    public class ChatUserController : ApiController
    {
       private IRepository<ChatUser> chatUserRepo { get; set; }

		public ChatUserController(IRepository<ChatUser> passedChatUserData)
		{
			this.chatUserRepo = passedChatUserData;
		}

		public ChatUserController()
			:this(new ChatUserRepository(new SessionStateContext()))
		{

		}

		[HttpGet]
		public IHttpActionResult GetAllChatUsers()
		{
			return Ok(this.chatUserRepo.All());
		}

		[HttpGet]
		public IHttpActionResult GetChatUser(string chatUserName)
		{
			return Ok(this.chatUserRepo.All().Where(x => x.UserName == chatUserName));
		}

		[HttpPost]
		public IHttpActionResult Save([FromBody]ChatUser chatUser)
		{
			if (this.IsValidChatUserToCreate(chatUser))
			{
				this.chatUserRepo.Save(chatUser);
				return Ok();
			}
			else
				return new StatusCodeResult(System.Net.HttpStatusCode.NotModified, this);
		}

		private bool IsValidChatUserToCreate(ChatUser chatUser)
		{
			return this.chatUserRepo.All().Count(x => x.UserName == chatUser.UserName) == 0;
		}
    }
}
