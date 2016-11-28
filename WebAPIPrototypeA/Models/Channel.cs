using System;
using System.Collections.Generic;

namespace Models
{
	public class Channel
	{
		public string ChannelName { get; set; }
		public List<ChatUser> Subscribers { get; set; }
	}
}
