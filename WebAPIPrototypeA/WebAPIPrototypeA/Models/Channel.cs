using System;
using System.Collections.Generic;

namespace WebAPIPrototypeA
{
	public class Channel
	{
		public string ChannelName { get; set; }
		public List<ChatUser> Subscribers { get; set; }
	}
}
