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
			ChatUserControllerTests tests = new ChatUserControllerTests();
			tests.SetUp();

			tests.GetChatUserByName();

			tests.CleanUp();
		}
	}
}
