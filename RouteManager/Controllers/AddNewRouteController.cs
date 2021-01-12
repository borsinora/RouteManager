using RouteManager.Data;
using RouteManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace RouteManager.Controllers
{
    public class AddNewRouteController : Controller
    {
        AddNewRouteModel addNewRouteModel = new AddNewRouteModel();
        AddNewRouteModel existingRouteModel = new AddNewRouteModel();
        EditRouteModel editRouteModel = new EditRouteModel();
        RoutesDatabaseEntities routeModel = new RoutesDatabaseEntities();

        // GET: Upload
        public ActionResult Index()
        {
            if (TempData["NewModel"] == null)
            {
                ViewBag.ShowList = false;
                AddNewRouteModel existingRoute = (AddNewRouteModel)TempData["ExistingRoute"];
                return View(existingRoute);
            }
            else
            {
                //List<NewRouteCoordinate> newRouteCoordinates = (List<NewRouteCoordinate>)TempData["NewRouteCoordinate"];
                ViewBag.ShowList = true;
                AddNewRouteModel addNewRoute = (AddNewRouteModel)TempData["NewModel"];
                //addNewRouteModel.NewCoordinates = newRouteCoordinates;
                //TempData["NewRouteCoordinate"] = newRouteCoordinates;
                return View(addNewRoute);
            }

        }
        public ActionResult EditRouteType(int? Id)
        {
            int id = 0;
            if (Id == null && TempData["EditIsDone"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (Id == null && TempData["EditIsDone"] != null)
            {
                return View(editRouteModel);
            }
            else
            {
                id = Id.Value;
                editRouteModel.SetRouteType(id);
                editRouteModel.TypeId = id;
                return View(editRouteModel);
            }
            
        }

        [HttpPost]
        public ActionResult AddNameAndDescription(int routeType, string name, string desc)
        {
            //int Id = int.Parse(routeType);
            routeModel.RouteTypes.First(t => t.Id == routeType).Name = name;
            routeModel.RouteTypes.First(t => t.Id == routeType).Description = desc;
            routeModel.SaveChanges();
            TempData["EditIsDone"] = true;
            return RedirectToAction("EditRouteType");
        }

        [HttpPost]
        public ActionResult UploadXML()
        {
            List<NewRouteCoordinate> newRouteCoordinates = new List<NewRouteCoordinate>();
            var xmlFile = Request.Files[0];
            if (xmlFile != null && xmlFile.ContentLength > 0)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlFile.InputStream);
                XmlNodeList coordNodes = xmlDocument.SelectNodes("Coordinates/Coordinate");

                foreach (XmlNode coord in coordNodes)
                {
                    newRouteCoordinates.Add(new NewRouteCoordinate()
                    {
                        X = decimal.Parse(coord["X"].InnerText, CultureInfo.InvariantCulture),
                        Y = decimal.Parse(coord["Y"].InnerText, CultureInfo.InvariantCulture),
                        Date = DateTime.ParseExact(coord["Date"].InnerText, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)
                    });
                }
                int isNew = IsTheSameAsOtherRoutes(newRouteCoordinates);
                if (isNew == -1)
                {
                    addNewRouteModel.NewCoordinates = newRouteCoordinates;
                    addNewRouteModel.AddToDatabase(newRouteCoordinates);
                    TempData["NewModel"] = addNewRouteModel;
                }
                else
                {
                    var route = routeModel.Routes.First(a => a.Id == isNew);
                    existingRouteModel.NewRoute = route;
                    TempData["ExistingRoute"] = existingRouteModel;
                }
            }
            return RedirectToAction("Index");
        }

        public int IsTheSameAsOtherRoutes(List<NewRouteCoordinate> nrcoordinates)
        {
            if (!routeModel.Routes.Any())
            {
                return -1;
            }
            else
            {
                foreach (Route r in routeModel.Routes)
                {
                    if (r.Coordinates.Count == nrcoordinates.Count)
                    {
                        List<Coordinate> routeCoordinates = r.Coordinates.OrderBy(d => d.Date).ToList();
                        bool isTheSame = true;
                        int j = 0;
                        while (j < r.Coordinates.Count)
                        {
                            if (nrcoordinates[j].X != routeCoordinates[j].X || nrcoordinates[j].Y != routeCoordinates[j].Y)
                            {
                                isTheSame = false;
                            }
                            j++;
                        }
                        if (isTheSame)
                        {
                            bool isTheSameDate = true;
                            int i = 0;
                            while (i < r.Coordinates.Count && isTheSameDate)
                            {
                                if (routeCoordinates[i].Date != nrcoordinates[i].Date)
                                {
                                    isTheSameDate = false;
                                }
                                i++;
                            }
                            if (isTheSameDate)
                            {
                                return r.Id;
                            }
                        }
                    }
                }
                return -1;
            }
        }





    }
}