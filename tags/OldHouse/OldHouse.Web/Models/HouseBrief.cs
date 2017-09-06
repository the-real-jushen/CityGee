using Jtext103.OldHouse.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    /// <summary>
    /// brif infor for houses, used for listing
    /// </summary>
    public class HouseBrief
    {
        public Guid  Id { get; set; }
        
        public string Name { get; set; }

        public string LocationString { get; set; }
        

        public string Abstarct { get; set; }

       
        public DateTime BuiltYear { get; set; }


        public string Cover { get; set; }


       
        public GeoPoint Location { get; set; }
        public string CodeName { get; set; }
        public string DistanceInKm { get; set; }
    }
}