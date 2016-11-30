using Microsoft.Owin.Cors;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalRChatServer.Startup))]
namespace SignalRChatServer
{
	public class Startup
	{
		public void Configuration(Owin.IAppBuilder app)
		{
			app.UseCors(CorsOptions.AllowAll);
			app.MapSignalR();
		}
	}
}
