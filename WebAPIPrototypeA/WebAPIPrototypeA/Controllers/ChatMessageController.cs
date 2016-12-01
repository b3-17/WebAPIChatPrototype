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

		[HttpGet]
		public IHttpActionResult GetChatMessages(string userToken, string channel)
		{
			// Get a generator + pagination working on this item to deliver huge chunks of message data
			IEnumerable<ChatMessage> allMessages = this.chatMessageRepo.All();

			List<ChatMessage> results = new List<ChatMessage>();

			if (!string.IsNullOrEmpty(userToken))
				results.AddRange(allMessages.OfType<DirectMessage>().Where(x => x.Originator.UserToken == userToken || x.To.UserToken == userToken));
			if(!string.IsNullOrEmpty(channel))
				results.AddRange(allMessages.OfType<ChannelMessage>().Where(x => x.Channel.ChannelName == channel));

			return Ok(results);
		}
	}
}