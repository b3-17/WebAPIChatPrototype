using System;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace WebAPIPrototypeA
{
	public class CacheControllerHandler : HttpControllerHandler, IRequiresSessionState
	{
		public CacheControllerHandler(RouteData routeData)
			: base(routeData)
		{ }
	}

	public class CacheStateRouteHandler : IRouteHandler
	{
		IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
		{
			return new CacheControllerHandler(requestContext.RouteData);
		}
	}
}
