using Microsoft.Owin.Cors;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(SignalRChatServer.Startup))]
namespace SignalRChatServer
{
	public class Startup
	{
		public void Configuration(Owin.IAppBuilder app)
		{
			app.Map("/signalr", map =>
			{
				map.UseCors(CorsOptions.AllowAll);
				var hubConfiguration = new HubConfiguration
				{
					EnableJSONP = true
				};

				hubConfiguration.EnableDetailedErrors = true;
				map.RunSignalR(hubConfiguration);
			});
		}
	}
}
