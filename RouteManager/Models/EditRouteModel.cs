using RouteManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteManager.Models
{
    public class EditRouteModel
    {
        public RoutesDatabaseEntities routeModel;
        public RouteType RouteType { get; set; }
        public int TypeId { get; set; }
        public EditRouteModel()
        {
            routeModel = new RoutesDatabaseEntities();
            RouteType = new RouteType();
        }
        public void SetRouteType(int id)
        {
            RouteType = routeModel.RouteTypes.First(a => a.Id == id);
            TypeId = id;
        }
    }
}