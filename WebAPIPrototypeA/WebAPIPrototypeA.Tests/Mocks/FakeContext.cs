using System;
using System.Collections.Generic;
using Models;
using Repository;

namespace WebAPIPrototypeA.Tests
{
	public class FakeContext : IContext
	{
		private IEnumerable<Channel> channels { get; set; }
		private IEnumerable<ChatUser> chatUsers { get; set; }
		private IEnumerable<ChatMessage> messages { get; set; }

		public FakeContext()
		{
			this.channels = new List<Channel>();
			this.chatUsers = new List<ChatUser>();
			this.messages = new List<ChatMessage>();
		}

		public IEnumerable<Channel> Channels
		{
			get
			{
				return this.channels;
			}

			set
			{
				this.channels = value;
			}
		}

		public IEnumerable<ChatMessage> ChatMessages
		{
			get
			{
				return this.messages;
			}

			set
			{
				this.messages = value;
			}
		}

		public IEnumerable<ChatUser> ChatUsers
		{
			get
			{
				return this.chatUsers;
			}

			set
			{
				this.chatUsers = value;
			}
		}
	}
}
