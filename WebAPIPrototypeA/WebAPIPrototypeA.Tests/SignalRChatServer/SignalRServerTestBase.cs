using System;
using Microsoft.AspNet.SignalR;
using Moq;
using System.Collections.Generic;

namespace WebAPIPrototypeA.Tests
{
	public class SignalRServerTestBase
	{
		private Mock<IConnection> mockConnection { get; set; }
		private Mock<System.Security.Principal.IPrincipal> mockUser { get; set; }
		private Mock<IRequest> mockRequest { get; set; }
		private Mock<IDictionary<string, Microsoft.AspNet.SignalR.Cookie>> mockCookies { get; set; }

		protected Mock<IRequest> BuildTestRequest()
		{ 
			this.mockUser = new Mock<System.Security.Principal.IPrincipal>();
			this.mockRequest = new Mock<IRequest>();
			this.mockCookies = new Mock<IDictionary<string, Cookie>>();
			this.mockRequest.Setup(x => x.User).Returns(mockUser.Object);
			this.mockRequest.Setup(x => x.Cookies).Returns(mockCookies.Object);

			return this.mockRequest;
		}

		protected void ClearTestRequest()
		{
			this.mockUser = null;
			this.mockCookies = null;
			this.mockRequest = null;
		}
	}
}
