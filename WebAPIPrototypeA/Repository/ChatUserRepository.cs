using System.Collections;
using System.Configuration;
using Models;
using System.Linq;
using System.Collections.Generic;

namespace Repository
{
	public class ChatUserRepository : IRepository<ChatUser>
	{
		private IContext context { get; set; }
		private IApplicationSettings applicationSettings { get; set; }

		public ChatUserRepository(IContext passedContext, IApplicationSettings passedApplicationSettings)
		{
			this.context = passedContext;
			this.applicationSettings = passedApplicationSettings;
		}

		public IEnumerable<ChatUser> All()
		{
			return this.context.ChatUsers;
		}

		public void Save(ChatUser itemToSave)
		{
			List<ChatUser> chatUsers = this.context.ChatUsers.ToList();
			if (chatUsers.Count() == 0)
				itemToSave.UserToken = this.applicationSettings.TokenBase;
			else
				itemToSave.UserToken = chatUsers.Max(x => x.UserToken) + 1;
			
			chatUsers.Add(itemToSave);
			this.context.ChatUsers = chatUsers;
		}
	}
}
