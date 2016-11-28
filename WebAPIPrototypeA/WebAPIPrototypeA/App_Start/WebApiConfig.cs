using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebAPIPrototypeA
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
			RouteTable.Routes.MapHttpRoute(
				name: "Subscribe",
				routeTemplate: "api/Channels/Subscribe",
				defaults: new { controller = "Channels", action = "Subscribe" }
			).RouteHandler = new SessionStateRouteHandler();

			RouteTable.Routes.MapHttpRoute(
				name: "Unsubscribe",
				routeTemplate: "api/Channels/Unsubscribe",
				defaults: new { controller = "Channels", action = "Unsubscribe" }
			).RouteHandler = new SessionStateRouteHandler();

			RouteTable.Routes.MapHttpRoute(
				name: "CreateChannel",
				routeTemplate: "api/CreateChannel/{channelName}",
				defaults: new { controller = "Channels", action = "Save", channelName = RouteParameter.Optional, method = "POST" }
			).RouteHandler = new SessionStateRouteHandler();


			RouteTable.Routes.MapHttpRoute(
				name: "DefaultChannel",
				routeTemplate: "api/{controller}/{channelName}",
				defaults: new { channelName = RouteParameter.Optional }
			).RouteHandler = new SessionStateRouteHandler();
		}
	}
}
