using System;
using Repository;

namespace WebAPIPrototypeA.Tests
{
	public class FakeApplicationSettings : IApplicationSettings
	{
		public int TokenBase
		{
			get
			{
				return 1000;
			}
		}
	}
}
