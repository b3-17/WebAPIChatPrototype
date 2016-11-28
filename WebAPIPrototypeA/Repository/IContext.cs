using System;
using System.Collections.Generic;
using Models;

namespace Repository
{
	public interface IContext
	{
		IEnumerable<Channel> Channels { get; set; }
	}
}
