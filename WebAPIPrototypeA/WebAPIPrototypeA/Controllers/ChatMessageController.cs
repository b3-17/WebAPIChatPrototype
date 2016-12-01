using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using Repository;
using Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System;

namespace WebAPIPrototypeA.Controllers
{
	public class ChatMessageController : ApiController
	{
		private IRepository<ChatMessage> chatMessageRepo { get; set; }

		public ChatMessageController(IRepository<ChatMessage> passedChatMessages)
		{
			this.chatMessageRepo = passedChatMessages;
		}

		public ChatMessageController()
			: this(new ChatMessageRepository(new SessionStateContext()))
		{

		}

		[HttpPost]
		public IHttpActionResult Save(HttpRequestMessage request)
		{
			var rawMessage = request.Content.ReadAsStringAsync().Result;

			JsonConverter[] chatConverters = { new ChatMessageConverter() };
			try
			{
				ChatMessage message = JsonConvert.DeserializeObject<ChatMessage>(rawMessage, new JsonSerializerSettings
				{
					Converters = chatConverters
				});
				this.chatMessageRepo.Save(message);
			}
			catch (Exception ex)
			{
				string mes = ex.Message;
			}

			return Ok();
		}
	}
}
