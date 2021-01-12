using RouteManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteManager.Models
{
    public class AddNewRouteModel
    {
        
        public RoutesDatabaseEntities routeModel;
        public List<NewRouteCoordinate> NewCoordinates { get; set; }
        public Route NewRoute { get; set; }
        public AddNewRouteModel()
        {
            NewCoordinates = new List<NewRouteCoordinate>();
            routeModel = new RoutesDatabaseEntities();
            NewRoute = new Route();
        }

        public void AddToDatabase(List<NewRouteCoordinate> nrc)
        {
            NewCoordinates = nrc;
            NewRoute.Duration = CalculateDuration(NewCoordinates);
            NewRoute.Length = CalculateLength(NewCoordinates);
            for (int i = 0; i < NewCoordinates.Count; i++)
            {
                Coordinate c = new Coordinate();
                c.X = NewCoordinates[i].X;
                c.Y = NewCoordinates[i].Y;
                c.Date = NewCoordinates[i].Date;
                NewRoute.Coordinates.Add(c);
            }
            int isSim = IsSimilarToOtheRoutes(NewCoordinates);
            RouteType routeType = new RouteType();
            if (isSim != -1)
            {
                //newRoute.RouteTypeId = isSim;
                routeType = routeModel.RouteTypes.First(a => a.Id == isSim);
                NewRoute.RouteType = routeType;
            }
            else
            {
                routeType.Name = "";
                routeType.Description = "";
                NewRoute.RouteType = routeType;
                routeModel.RouteTypes.Add(routeType);
            }
            
            routeModel.Routes.Add(NewRoute);
            routeModel.SaveChanges();
        }

        public TimeSpan CalculateDuration(List<NewRouteCoordinate> nrc)
        {
            TimeSpan ts = nrc.Last().Date.Subtract(nrc.First().Date);
            //r.Duration = ts;
            return ts;
        }
        public decimal CalculateLength(List<NewRouteCoordinate> nrc)
        {
            double length = 0;
            for (int i = 0; i < nrc.Count-1; i++)
            {
                length += Math.Sqrt(decimal.ToDouble((nrc[i].X - nrc[i + 1].X) * (nrc[i].X - nrc[i + 1].X) + (nrc[i].Y - nrc[i + 1].Y) * (nrc[i].Y - nrc[i + 1].Y)));
            }
            return (decimal)length*5;
        }
        public int IsSimilarToOtheRoutes(List<NewRouteCoordinate> nrcoordinates)
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
                        decimal epsilon = 0.005M;
                        bool isSimilar = true;
                        int j = 0;
                        while (j < r.Coordinates.Count)
                        {
                            if ((nrcoordinates[j].X < routeCoordinates[j].X - epsilon 
                                || nrcoordinates[j].X > routeCoordinates[j].X + epsilon) 
                                || (nrcoordinates[j].Y < routeCoordinates[j].Y - epsilon 
                                || nrcoordinates[j].Y > routeCoordinates[j].Y + epsilon))
                            {
                                isSimilar = false;
                            }
                            j++;
                        }
                        if (isSimilar)
                        {
                            return r.RouteTypeId;
                        }
                    }
                }
                return -1;
            }
        }

    }
}