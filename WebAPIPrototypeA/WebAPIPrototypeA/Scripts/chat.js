; chat = {
	chatHub:	'',

	startUpChat: function(){
		$.connection.hub.url = "http://localhost:8081/signalr"; 
		chat.chatHub = $.connection.chatHub;
		console.log(chat.chatHub.ConnectionId);
		// if the listeners are not called before hub.start(), chat hub will NOT initialise
		chat.registerListeners();

	    //({ transport: ['webSockets', 'serverSentEvents', 'longPolling'] })
		$.connection.hub.start().done(function(data){
			chat.storeUserTokenId(data.id);
			chat.registerChannelBroadcastEvent();
			chat.registerDirectMessageEvent();
			//chat.registerJoiningChannelEvent();
       });
    },

    storeUserTokenId: function(id){
    	document.getElementById('userConnectionToken').value = id;
    },

    registerListeners: function(){
    	chat.chatHub.client.sendChannelMessage = function (fromUserName, message) {
    		chat.populateChatHistory('broadcast from: ' + fromUserName, message);
    	}

    	chat.chatHub.client.sendUserCreatedMessage = function (message) {
    		chat.populateChatHistory(message, '');
    		spikeRunner.updateUserList();
    	}

    	chat.chatHub.client.sendChannelJoiningMessage = function (userJoiningName, message) {
    		chat.populateChatHistory('broadcast from: ' + userJoiningName, message);
    	}

   		chat.chatHub.client.sendChannelCreationMessage = function (channel, fromUserName) {
   			chat.populateChatHistory('a new channel - ' + channel + ' was just created by ', fromUserName);
   			spikeRunner.updateChannelLists();
   		}

   		chat.chatHub.client.sendDirectMessage = function (fromUserName, message) {
   			chat.populateChatHistory(message, 'direct from: ' + fromUserName);
   		}
    },

    registerChannelBroadcastEvent: function(){
    	$('#sendMessageToChannel').on('click', function(){
			chat.chatHub.server.channelBroadCast(spikeRunner.getSelectedFromDropDown('channels'), 
			spikeRunner.getMyName(), document.getElementById('messageToSendToChannel').value);
		});
    },

    registerDirectMessageEvent: function(){
    	$('#sendMessageToUser').on('click', function(){
    		chat.chatHub.server.directMessageUser(spikeRunner.getSelectedFromDropDown('users'), 
    		spikeRunner.getMyName(), document.getElementById('messageToSendToUser').value);
    	});
    },

    registerJoiningChannelEvent: function(){
    	$('#joinChannel').on('click', function(){
    	console.log('hi');
    		chat.chatHub.server.joinChatChannel(spikeRunner.getSelectedFromDropDown('channelsToJoin'),
    		spikeRunner.getUserToken(), spikeRunner.getMyName());
    	});
    },

    populateChatHistory: function(userName, chatMessage){
    	document.getElementById('chatResults').innerHTML += userName + ':  ' + chatMessage + '<br/>';
    },
}
