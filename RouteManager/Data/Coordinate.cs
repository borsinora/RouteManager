//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RouteManager.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Coordinate
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual Route Route { get; set; }

        //decimal.Round(AverageVelocity, 2, MidpointRounding.AwayFromZero);
        public override string ToString()
        {
            return $"({decimal.Round(X, 2, MidpointRounding.AwayFromZero)}; {decimal.Round(Y, 2, MidpointRounding.AwayFromZero)})";
        }
    }
}
