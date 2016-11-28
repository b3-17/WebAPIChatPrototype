using System;
using System.Configuration;

namespace Repository
{
	public class ApplicationSettings : IApplicationSettings
	{
		public ApplicationSettings()
		{
		}

		public int TokenBase
		{
			get
			{
				return int.Parse(ConfigurationManager.AppSettings["TokenBase"].ToString());
			}
		}
	}
}
