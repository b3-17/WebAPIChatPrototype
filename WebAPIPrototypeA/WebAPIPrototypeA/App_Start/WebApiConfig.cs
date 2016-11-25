using System.Net.Http.Headers;
using System.Web.Http;

namespace WebAPIPrototypeA
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

			config.Routes.MapHttpRoute(
				name: "Subscription",
				routeTemplate: "api/Channels/Subscribe",
				defaults: new { controller = "Channels", action = "Subscribe" }
			);

			config.Routes.MapHttpRoute(
				name: "Subscription",
				routeTemplate: "api/Channels/Unsubscribe",
				defaults: new { controller = "Channels", action = "Unsubscribe" }
			);

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{channelName}",
				defaults: new { channelName = RouteParameter.Optional }
			);
		}
	}
}
