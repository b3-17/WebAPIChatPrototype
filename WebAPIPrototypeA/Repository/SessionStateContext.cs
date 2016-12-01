using System;
using System.Collections.Generic;
using System.Web;
using Models;

namespace Repository
{
	public class SessionStateContext : IContext
	{
		private string ChannelsSessionKey { get { return "Channels"; } }
		private string ChatUsersSessionKey { get { return "ChatUsers"; } }
		private string ChatMessagesSessionKey { get { return "ChatMessages";}}

		public IEnumerable<Channel> Channels
		{
			get
			{
				if (HttpContext.Current.Session[this.ChannelsSessionKey] == null)
					HttpContext.Current.Session[this.ChannelsSessionKey] = new List<Channel>();
				return HttpContext.Current.Session[this.ChannelsSessionKey] as List<Channel>;
			}
			set
			{
				if (HttpContext.Current.Session[this.ChannelsSessionKey] == null)
					HttpContext.Current.Session[this.ChannelsSessionKey] = new List<Channel>();
				HttpContext.Current.Session[this.ChannelsSessionKey] = value;
			}
		}

		public IEnumerable<ChatUser> ChatUsers
		{
			get
			{
				if (HttpContext.Current.Session[this.ChatUsersSessionKey] == null)
					HttpContext.Current.Session[this.ChatUsersSessionKey] = new List<ChatUser>();
				return HttpContext.Current.Session[this.ChatUsersSessionKey] as List<ChatUser>;
			}

			set
			{
				if (HttpContext.Current.Session[this.ChatUsersSessionKey] == null)
					HttpContext.Current.Session[this.ChatUsersSessionKey] = new List<ChatUser>();
				HttpContext.Current.Session[this.ChatUsersSessionKey] = value;
			}
		}

		public IEnumerable<ChatMessage> ChatMessages
		{
			get
			{
				if (HttpContext.Current.Session[this.ChatMessagesSessionKey] == null)
					HttpContext.Current.Session[this.ChatMessagesSessionKey] = new List<ChatMessage>();
				return HttpContext.Current.Session[this.ChatMessagesSessionKey] as List<ChatMessage>;
			}

			set
			{
				if (HttpContext.Current.Session[this.ChatMessagesSessionKey] == null)
					HttpContext.Current.Session[this.ChatMessagesSessionKey] = new List<ChatMessage>();
				HttpContext.Current.Session[this.ChatMessagesSessionKey] = value;
			}
		}
	}
}
