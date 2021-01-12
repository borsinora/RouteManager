using RouteManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteManager.Models
{
    public class DrawImageModel
    {
        RoutesDatabaseEntities routeModel;

        public DrawImageModel()
        {
            routeModel = new RoutesDatabaseEntities();
            AllRoutes = new List<Route>();
            RouteTypeNames = routeModel.RouteTypes.Select(a => a.Name).ToList();
            RouteType = new RouteType();
            RouteSectionInfos = new List<RouteSectionInfo>();
        }
        public List<RouteSectionInfo> RouteSectionInfos { get; set; }
        public decimal AverageLength { get; set; }
        public TimeSpan BestTime { get; set; }
        public decimal AverageVelocity { get; set; }
        public RouteType RouteType { get; set; }
        public bool ShowAll { get; set; }
        public List<string> RouteTypeNames { get; set; }
        public int RouteTypeId { get; set; }
        public List<Route> AllRoutes { get; set; }
        public List<Coordinate> SelectedCoordinates { get; set; }

        public void SetRouteTypeValues(int id)
        {
            RouteType = routeModel.RouteTypes.First(a => a.Id == id);
            AllRoutes = routeModel.Routes.Where(r => r.RouteTypeId == id).ToList();
            SelectedCoordinates = routeModel.Routes.First(r => r.RouteTypeId == id).Coordinates.ToList();
            SetBestTime();
            //decimal.Round(AverageVelocity, 2, MidpointRounding.AwayFromZero);
            AverageLength = decimal.Round((AllRoutes.Sum(a => a.Length) / AllRoutes.Count), 2, MidpointRounding.AwayFromZero);
            SetAverageVelocity();
            SetRouteSectionInfo();
        }

        public void SetRouteSectionInfo()
        {
            for (int i = 0; i < SelectedCoordinates.Count-1; i++)
            {
                Coordinate p1 = SelectedCoordinates[i];
                Coordinate p2 = SelectedCoordinates[i + 1];
                RouteSectionInfo routeSectionInfo = new RouteSectionInfo
                {
                    //decimal.Round(AverageVelocity, 2, MidpointRounding.AwayFromZero);
                    FirstCoordinate = p1,
                    SecondCoordinate = p2,
                    SectionTime = p2.Date.Subtract(p1.Date),
                    SectionLength = decimal.Round(5*(decimal)Math.Sqrt(decimal.ToDouble((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y))), 2, MidpointRounding.AwayFromZero)
                };
                routeSectionInfo.SectionVelocity = decimal.Round(routeSectionInfo.SectionLength / ((decimal)routeSectionInfo.SectionTime.TotalHours), 2, MidpointRounding.AwayFromZero);
                RouteSectionInfos.Add(routeSectionInfo);
            }
        }

        public void SetBestTime()
        {
            BestTime = AllRoutes[0].Duration;
            foreach (Route r in AllRoutes)
            {
                if (r.Duration.CompareTo(BestTime) < 0)
                {
                    BestTime = r.Duration;
                }
            }
        }
        public void SetAverageVelocity()
        {
            decimal allLength = 0;
            double allHours = 0;
            foreach (Route r in AllRoutes)
            {
                allLength += r.Length;
                allHours += r.Duration.TotalHours;
            }
            AverageVelocity = decimal.Round((allLength / (decimal)allHours), 2, MidpointRounding.AwayFromZero);
        }
        public List<Coordinate> CalculateCoordinates(List<Coordinate> calcCoord)
        {
            decimal smallestX = calcCoord[0].X;
            decimal smallestY = calcCoord[0].Y;
            List<Coordinate> coordinatesToBeDrawn = new List<Coordinate>();
            foreach (Coordinate c in calcCoord)
            {
                Coordinate copy = new Coordinate();
                copy.X = c.X;
                copy.Y = c.Y;
                copy.Date = c.Date;
                coordinatesToBeDrawn.Add(copy);
                if (c.X < smallestX)
                {
                    smallestX = c.X;
                }
                if (c.Y < smallestY)
                {
                    smallestY = c.Y;
                }
            }
            decimal biggestModifiedX = -1;
            decimal biggestModifiedY = -1;
            foreach (Coordinate c in coordinatesToBeDrawn)
            {
                c.X -= smallestX;
                c.Y -= smallestY;
                if (c.X > biggestModifiedX)
                {
                    biggestModifiedX = c.X;
                }
                if (c.Y > biggestModifiedY)
                {
                    biggestModifiedY = c.Y;
                }
            }
            decimal projectionValue;
            if (biggestModifiedX != 0 && biggestModifiedY != 0)
            {
                if ((960 / biggestModifiedX) > (520 / biggestModifiedY))
                {
                    projectionValue = 520 / biggestModifiedY;
                }
                else
                {
                    projectionValue = 960 / biggestModifiedX;
                }
                foreach (Coordinate c in coordinatesToBeDrawn)
                {
                    c.X *= projectionValue;
                    c.Y *= projectionValue;
                    c.Y = 520 - c.Y;
                    c.X += 20;
                    c.Y += 20;
                }
            }
            return coordinatesToBeDrawn;
        }

    }
}