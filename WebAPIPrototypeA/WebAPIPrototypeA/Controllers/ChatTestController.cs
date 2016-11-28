using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAPIPrototypeA.Controllers
{
	public class ChatTestController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}
