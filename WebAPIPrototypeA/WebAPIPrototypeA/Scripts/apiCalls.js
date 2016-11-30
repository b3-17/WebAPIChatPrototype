
initApiEvents = { 
	init: function(){
		$('#newChannel').on('click', function(){
			var channelData = {ChannelName: document.getElementById('newChannelName').value, 
			Subscribers:[{ UserName:  spikeRunner.getMyName(), UserToken: spikeRunner.getUserToken() }]}
			apiCalls.newChannel(spikeRunner.updateChannelLists, channelData);
			chat.chatHub.server.createChatChannel(channelData.ChannelName, spikeRunner.getMyName());
		});
		
		$('#subscribe').click(function(){
			var subscribeData = {ChannelName: spikeRunner.getSelectedFromDropDown('channelsToJoin'), Subscribers:
			[{ UserName: spikeRunner.getMyName(), UserToken: spikeRunner.getUserToken() }]};
			apiCalls.subscribe(spikeRunner.updateChannelLists, subscribeData);
    		chat.chatHub.server.joinChatChannel(spikeRunner.getSelectedFromDropDown('channelsToJoin'),
    		spikeRunner.getUserToken(), spikeRunner.getMyName());
		});

		$('#unsubscribe').click(function(){
			var unsubData = {ChannelName: spikeRunner.getSelectedFromDropDown('channelsToLeave'), 
			Subscribers:[{ UserName: spikeRunner.getMyName(), UserToken: spikeRunner.getUserToken()}]};
			apiCalls.unsubscribe(spikeRunner.updateChannelLists, unsubData);
			chat.chatHub.server.leaveChatChannel(spikeRunner.getSelectedFromDropDown('channelsToLeave'),
    		spikeRunner.getUserToken(), spikeRunner.getMyName());
		});
		
		$('#getChannels').click(function(){
			apiCalls.getChannels(displayChannelData);
			function displayChannelData(data){
				document.getElementById('channelListDetails').innerHTML = JSON.stringify(data);
			}
		});

		$('#getSpecificChannel').click(function(){
			apiCalls.getChannelByName(spikeRunner.updateChannelLists, document.getElementById('channelName').value);
		});

		$('#createUser').click(function(){
			var user = {UserName: document.getElementById('userName').value, UserToken: spikeRunner.getUserToken()};
			document.getElementById('myName').value = user.UserName;
			apiCalls.createUser(spikeRunner.updateUserList, user);
			chat.chatHub.server.createUser(user.UserName);
		});
	}
}

; apiCalls = {
	newChannel: function(callback, channelData){
		var get = new getData('http://127.0.0.1:8080/api/createchannel');
		get.postForJsonResponse(JSON.stringify(channelData), callback);//);
	},

	subscribe: function(callback, subscriptionData){
		var get = new getData('http://127.0.0.1:8080/api/channels/subscribe');
		get.postForJsonResponse(JSON.stringify(subscriptionData), callback);//);
	},

	unsubscribe: function(callback, unsubData){
		var get = new getData('http://127.0.0.1:8080/api/channels/unsubscribe');
		get.postForJsonResponse(JSON.stringify(unsubData), callback);
	},

	getChannels: function(callback){
		var get = new getData('http://127.0.0.1:8080/api/channels');
		get.getForJsonResponse(callback);
	},

	getChannelByName: function(callback, channelName){
		var get = new getData('http://127.0.0.1:8080/api/channels/' + channelName);;
		get.getForJsonResponse(callback);
	},

	createUser: function(callback, userData){
		var get = new getData('http://127.0.0.1:8080/api/chatuser');
		get.postForJsonResponse(JSON.stringify(userData), callback);
	},
}