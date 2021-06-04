using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestSharp;


namespace AIS_REST.Controllers
{
    public class HomeController : Controller
    {
        Updater updater;
        public ActionResult Index()
        {
            updater = new Updater();
            updater.Start();
            return View();
        }
    }
}
