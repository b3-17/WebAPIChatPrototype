using System;
using Models;
using System.Linq;
using System.Collections.Generic;

namespace Repository
{
	public class ChatUserRepository : IRepository<ChatUser>
	{
		private IContext context { get; set; }

		public ChatUserRepository(IContext passedContext)
		{
			this.context = passedContext;
		}

		public IEnumerable<ChatUser> All()
		{
			return this.context.ChatUsers;
		}

		public void Save(ChatUser itemToSave)
		{
			List<ChatUser> chatUsers = this.context.ChatUsers.ToList();
			chatUsers.Add(itemToSave);
			this.context.ChatUsers = chatUsers;
		}
	}
}
