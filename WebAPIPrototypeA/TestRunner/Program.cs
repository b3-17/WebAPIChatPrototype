using System;
using WebAPIPrototypeA.Tests;

namespace TestRunner
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			//ChannelRepositoryTests tests = new ChannelRepositoryTests();
			//ChannelsTests tests = new ChannelsTests();
			//ChatUserRepositoryTests tests = new ChatUserRepositoryTests();
			//ChatUserControllerTests tests = new ChatUserControllerTests();
			//ChatHubTests tests = new ChatHubTests();
			ChatMessageControllerTests tests = new ChatMessageControllerTests();

			tests.SetUp();

			tests.SaveChatMessage();

			tests.CleanUp();
		}
	}
}
