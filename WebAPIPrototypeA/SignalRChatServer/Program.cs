using System;
using Microsoft.Owin.Hosting;

namespace SignalRChatServer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			using (WebApp.Start<Startup>("http://+:8081/"))
			{
				Console.WriteLine("Server running at http://localhost:8081/");
				Console.ReadLine();
			}
		
		}
	}
}
