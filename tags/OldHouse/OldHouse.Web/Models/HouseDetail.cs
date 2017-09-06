using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    public class HouseDetail:HouseBrief
    {
        public string Description { get; set; }

        public string Condition { get; set; }


        public double HistoricValue { get; set; }
        public double PhotoValue { get; set; }


        public List<string> Images { get; set; }
        public List<string> Tags { get; set; }



        public double OverallValue
        {
            get;
            set;
        }

    }
}