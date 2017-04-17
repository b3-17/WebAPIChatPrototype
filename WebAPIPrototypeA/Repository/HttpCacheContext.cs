using System;
using System.Collections.Generic;
using System.Web;
using Models;

namespace Repository
{
	public class HttpCacheContext : IContext
	{
		private string ChannelsCacheKey { get { return "Channels"; } }
		private string ChatUsersCacheKey { get { return "ChatUsers"; } }
		private string ChatMessagesCacheKey { get { return "ChatMessages";}}

		public IEnumerable<Channel> Channels
		{
			get
			{
				var x = HttpRuntime.Cache;
				if (HttpRuntime.Cache[this.ChannelsCacheKey] == null)
					HttpRuntime.Cache.Insert(this.ChannelsCacheKey, new List<Channel>());
				return HttpContext.Current.Cache[this.ChannelsCacheKey] as List<Channel>;
			}
			set
			{
				if (HttpContext.Current.Cache[this.ChannelsCacheKey] == null)
					HttpContext.Current.Cache[this.ChannelsCacheKey] = new List<Channel>();
				HttpContext.Current.Cache[this.ChannelsCacheKey] = value;
			}
		}

		public IEnumerable<ChatUser> ChatUsers
		{
			get
			{
				if (HttpContext.Current.Cache[this.ChatUsersCacheKey] == null)
					HttpContext.Current.Cache[this.ChatUsersCacheKey] = new List<ChatUser>();
				return HttpContext.Current.Cache[this.ChatUsersCacheKey] as List<ChatUser>;
			}

			set
			{
				if (HttpContext.Current.Cache[this.ChatUsersCacheKey] == null)
					HttpContext.Current.Cache[this.ChatUsersCacheKey] = new List<ChatUser>();
				HttpContext.Current.Cache[this.ChatUsersCacheKey] = value;
			}
		}

		public IEnumerable<ChatMessage> ChatMessages
		{
			get
			{
				if (HttpContext.Current.Cache[this.ChatMessagesCacheKey] == null)
					HttpContext.Current.Cache[this.ChatMessagesCacheKey] = new List<ChatMessage>();
				return HttpContext.Current.Cache[this.ChatMessagesCacheKey] as List<ChatMessage>;
			}

			set
			{
				if (HttpContext.Current.Cache[this.ChatMessagesCacheKey] == null)
					HttpContext.Current.Cache[this.ChatMessagesCacheKey] = new List<ChatMessage>();
				HttpContext.Current.Cache[this.ChatMessagesCacheKey] = value;
			}
		}
	}
}
