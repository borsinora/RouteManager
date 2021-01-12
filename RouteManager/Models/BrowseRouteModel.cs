using RouteManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteManager.Models
{
    public class BrowseRouteModel
    {
        RoutesDatabaseEntities routeModel;

        public BrowseRouteModel()
        {
            routeModel = new RoutesDatabaseEntities();
            RouteTypes = routeModel.RouteTypes.ToList();
            
        }
        public List<RouteType> RouteTypes { get; set; }



    }
}