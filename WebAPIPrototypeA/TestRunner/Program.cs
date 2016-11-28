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
			ChatUserRepositoryTests tests = new ChatUserRepositoryTests();
			tests.SetUp();

			tests.SaveUser();

			tests.CleanUp();
		}
	}
}
