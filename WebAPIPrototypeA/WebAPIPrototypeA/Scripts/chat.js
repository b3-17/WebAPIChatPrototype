; chat = {
	chatHub:	'',

	startUpChat: function(){
		$.connection.hub.url = "http://localhost:8081/signalr/hubs"; 
		$.connection.hub.logging = true;
		chat.chatHub = $.connection.chatHub;
		// if the listeners are not called before hub.start(), chat hub will NOT initialise
		chat.registerListeners();

	    //({ transport: ['webSockets', 'serverSentEvents', 'longPolling'] })
		$.connection.hub.start({ transport: ['webSockets', 'serverSentEvents', 'longPolling', 'foreverFrame'] }).done(function(data){
			chat.storeUserTokenId(data.id);
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
    		spikeRunner.updateChannelLists();
    	}

    	chat.chatHub.client.sendChannelLeavingMessage = function (userLeavingName, message) {
    		chat.populateChatHistory('broadcast from: ' + userLeavingName, message);
    	}

   		chat.chatHub.client.sendChannelCreationMessage = function (channel, fromUserName) {
   			chat.populateChatHistory('a new channel - ' + channel + ' was just created by ', fromUserName);
   			spikeRunner.updateChannelLists();
   		}

   		chat.chatHub.client.sendDirectMessage = function (fromUserName, message) {
   			chat.populateChatHistory(message, 'direct from: ' + fromUserName);
   		}
    },

    populateChatHistory: function(userName, chatMessage){
    	document.getElementById('chatResults').innerHTML += userName + ':  ' + chatMessage + '<br/>';
    },
}
