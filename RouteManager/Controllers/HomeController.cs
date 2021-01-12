using RouteManager.Data;
using RouteManager.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace RouteManager.Controllers
{
    public class HomeController : Controller
    {
        
        public BrowseRouteModel browseRouteModel;

        public HomeController()
        {
            browseRouteModel = new BrowseRouteModel();
        }


        public ActionResult Index()
        {
            return View(browseRouteModel);
        }
    }
}