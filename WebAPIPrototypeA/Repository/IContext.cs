﻿using System;
using System.Collections.Generic;
using Models;

namespace Repository
{
	public interface IContext
	{
		IEnumerable<Channel> Channels { get; set; }
		IEnumerable<ChatUser> ChatUsers { get; set;}
		IEnumerable<ChatMessage> ChatMessages { get; set; }
	}
}
