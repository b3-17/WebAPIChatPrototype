using System;
namespace Models
{
	public class DirectMessage : ChatMessage
	{
		public ChatUser To { get; set; }
	}
}
