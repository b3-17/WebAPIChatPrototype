using System;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace WebAPIPrototypeA
{
	public class SessionableControllerHandler : HttpControllerHandler, IRequiresSessionState
	{
		public SessionableControllerHandler(RouteData routeData)
			: base(routeData)
		{ }
	}

	public class SessionStateRouteHandler : IRouteHandler
	{
		IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
		{
			return new SessionableControllerHandler(requestContext.RouteData);
		}
	}

	public class testGuy
	{
		public void SaveStuff(string thing)
		{
			HttpContext.Current.Session["I"] = thing;
		}
	}
}
