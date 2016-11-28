using System.Web;

namespace WebAPIPrototypeA.Tests
{
	public class RepositoryTestBase
	{
		protected void SetUpFakeHttpSessionMock(string url)
		{
			HttpContext.Current = StaticHttpMock.FakeHttpContext(url);
		}
	}
}
