using RouteManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteManager.Models
{
    public class RouteSectionInfo
    {
        public RouteSectionInfo()
        {
            SectionTime = new TimeSpan();
            FirstCoordinate = new Coordinate();
            SecondCoordinate = new Coordinate();
        }
        public decimal SectionLength { get; set; }
        public decimal SectionVelocity { get; set; }
        public TimeSpan SectionTime { get; set; }
        public Coordinate FirstCoordinate { get; set; }
        public Coordinate SecondCoordinate { get; set; }
    }
}