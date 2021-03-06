; spikeRunner = {
		updateChannelLists: function(){
			var get = new getData('http://127.0.0.1:8080/api/channels');
			get.getForJsonResponse(setChannelDropdown);
			function setChannelDropdown(data){	
				var channelLists = ["channels", "channelsToJoin", "channelsToLeave"];
				for(var i = 0; i < channelLists.length; i++){
					var select = document.getElementById(channelLists[i]);
					$("#" + channelLists[i]).empty();
					var parsedData = JSON.parse(data);

					for( var y = 0; y < parsedData.length; y++){
						var option = document.createElement('option');
						option.value = parsedData[y].ChannelName;
						option.innerHTML = parsedData[y].ChannelName;
						select.appendChild(option);
				}
			}
		}
	},
	
	getSelectedFromDropDown: function(listId){
		var dropdown = document.getElementById(listId);
		if(dropdown.length > 0)
			return dropdown[dropdown.selectedIndex].value;
		else
			return '';
	},


	getSelectedTextFromDropDown: function(listId){
		var dropdown = document.getElementById(listId);
		if(dropdown.length > 0)
			return dropdown[dropdown.selectedIndex].innerHTML;
		else
			return '';
	},

	updateUserList :function(){
		var get = new getData('http://127.0.0.1:8080/api/chatuser');
		get.getForJsonResponse(setUserDropdown);
		function setUserDropdown(data){
			var parsedData = JSON.parse(data);
			$('#users').empty();
			var select = document.getElementById('users');
			for( var i = 0; i < parsedData.length; i++){
				var option = document.createElement('option');
				option.value = parsedData[i].UserToken;
				option.innerHTML = parsedData[i].UserName;
				select.appendChild(option);
			}
		}
	},

	getUserToken: function(){
		return document.getElementById('userConnectionToken').value;
	},

	getMyName: function(){
		return document.getElementById('myName').value;
	}
}