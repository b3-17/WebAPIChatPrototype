using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRChatServer
{
	[HubName("chatHub")]
	public class ChatHub : Hub
	{
		public void CreateUser(string userCreatedName)
		{
			string userCreatedMessage = String.Format("A user has signed up: {0}", userCreatedName);
			Clients.All.sendUserCreatedMessage(userCreatedMessage);
		}

		public void CreateChatChannel(string channel, string fromUserName) 
		{
			Groups.Add(Context.ConnectionId, channel);
			Clients.All.sendChannelCreationMessage(channel, fromUserName);
		}

		public void JoinChatChannel(string channel, string userJoiningId, string userJoiningName)
		{ 
			Groups.Add(userJoiningId, channel);
			string message = String.Format("{0} joined {1}", userJoiningName, channel);
			Clients.Group(channel).sendChannelJoiningMessage(userJoiningName, message);
		}

		public void LeaveChatChannel(string channel, string userJoiningId, string userJoiningName)
		{
			Groups.Remove(userJoiningId, channel);
			string message = String.Format("{0} left {1}", userJoiningName, channel);
			Clients.Group(channel).sendChannelLeavingMessage(userJoiningName, message);
		}

		public void ChannelBroadCast(string channel, string fromUserName, string message)
		{
		 	Clients.Group(channel).sendChannelMessage(fromUserName, message);
		}

		public void DirectMessageUser(string userId, string fromUserName, string message)
		{
			Clients.Client(userId).sendDirectMessage(fromUserName, message);
		}
	}
}
