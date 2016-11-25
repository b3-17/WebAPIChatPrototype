using System;
using WebAPIPrototypeA.Tests;

namespace TestRunner
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			ChannelsTests tests = new ChannelsTests();
			tests.SetUp();

			tests.UnsubscribeUserNotSubscribedFromExistentChannel();

			tests.CleanUp();
		}
	}
}
