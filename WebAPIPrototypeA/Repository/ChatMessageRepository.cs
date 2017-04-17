using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace Repository
{
	public class ChatMessageRepository : IRepository<ChatMessage>
	{
		private IContext context { get; set; }

		public ChatMessageRepository(IContext passedContext)
		{
			this.context = passedContext;
		}

		public void Save(ChatMessage itemToSave)
		{
			List<ChatMessage> chatMessages = this.context.ChatMessages.ToList();
			itemToSave.ServerTimeDateSent = DateTime.Now;
			chatMessages.Add(itemToSave);
			this.context.ChatMessages = chatMessages;
		}

		public IEnumerable<ChatMessage> All()
		{
			
			return this.context.ChatMessages;
		}
	}
}
