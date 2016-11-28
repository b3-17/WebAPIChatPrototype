using System;
using System.Collections.Generic;
using System.Web;
using Models;

namespace Repository
{
	public class SessionStateContext : IContext
	{
		public string ChannelsSessionKey { get { return "Channels"; } }

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
	}
}
