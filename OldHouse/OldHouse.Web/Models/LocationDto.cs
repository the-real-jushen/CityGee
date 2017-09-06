using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    public class LocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string IpAddress { get; set; }

        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }

        public LocationDto()
        {
            Country = "";
            Province = "";
            City = "";
        }
    }
}