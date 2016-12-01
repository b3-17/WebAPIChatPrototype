using System;
namespace Models
{
	public abstract class ChatMessage
	{
		public string Message { get; set; }
		public ChatUser Originator { get; set; }
		public DateTime ServerTimeDateSent { get; set; }
	}
}
