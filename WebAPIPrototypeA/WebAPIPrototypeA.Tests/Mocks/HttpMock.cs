using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using Moq;
using System.IO;
using System.Web.SessionState;

namespace WebAPIPrototypeA.Tests
{
	public class HttpMock
	{
		private Dictionary<string, object> sessionContainer { get; set; }

		public HttpMock()
		{
			this.sessionContainer = new Dictionary<string, object>();
		}

		public Mock<HttpContextBase> MockContext()
		{
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			var response = new Mock<HttpResponseBase>();
			var session = new Mock<HttpSessionStateBase>();
			var server = new Mock<HttpServerUtilityBase>(MockBehavior.Loose);
			var files = new Mock<HttpFileCollectionBase>();
			var cookies = new HttpCookieCollection();
			var applicationInstance = new Mock<HttpApplication>();
			var applicationBase = new Mock<HttpApplicationStateBase>();

			request.Setup(x => x.Cookies).Returns(cookies);
			request.Setup(x => x.Files).Returns(files.Object);
			response.Setup(x => x.Cookies).Returns(cookies);
			context.Setup(x => x.ApplicationInstance).Returns(applicationInstance.Object);
			context.Setup(x => x.Application).Returns(applicationBase.Object);
			context.Setup(x => x.ApplicationInstance).Returns(applicationInstance.Object);
			context.Setup(x => x.Request).Returns(request.Object);
			context.Setup(x => x.Response).Returns(response.Object);
			context.Setup(x => x.Session).Returns(session.Object);
			context.Setup(x => x.Server).Returns(server.Object);
			context.Setup(x => x.Cache).Returns(new System.Web.Caching.Cache());

			session.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<object>())).Callback((string key, object value) => this.SetSessionVariable(key, value));
			session.Setup(x => x.Remove(It.IsAny<string>())).Callback((string key) => this.RemoveSessionVariable(key));
			session.Setup(x => x[It.IsAny<string>()]).Returns((string key) => this.GetSessionIndexVariable(key));

			return context;
		}

		private void SetSessionVariable(string key, object value)
		{
			if (!this.sessionContainer.Keys.Contains(key))
				this.sessionContainer.Add(key, value);
			else throw new Exception("This key already exists - for safety, please remove it and add it again rather than reset over it");
		}

		private void RemoveSessionVariable(string key)
		{
			if (this.sessionContainer.Keys.Contains(key))
				this.sessionContainer.Remove(key);
		}

		private object GetSessionIndexVariable(string key)
		{
			if (this.sessionContainer.Keys.Contains(key))
				return this.sessionContainer[key];
			else return null;
		}
	}

	public static class StaticHttpMock
	{ 
		public static HttpContext FakeHttpContext(string url)
		{
			var uri = new Uri(url);
			var httpRequest = new HttpRequest(string.Empty, uri.ToString(), uri.Query.TrimStart('?'));
			var stringWriter = new StringWriter();
			var httpResponse = new HttpResponse(stringWriter);
			var httpContext = new HttpContext(httpRequest, httpResponse);

			var sessionContainer = new HttpSessionStateContainer("id",
											new SessionStateItemCollection(),
											new HttpStaticObjectsCollection(),
											10, true, HttpCookieMode.AutoDetect,
											SessionStateMode.InProc, false);

			SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);

			return httpContext;
		}

	}
}
