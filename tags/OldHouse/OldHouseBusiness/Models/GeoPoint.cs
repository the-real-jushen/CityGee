using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.OldHouse.Business.Models
{
    public class GeoPoint
    {
        
        //currently we assume all the coordinates are GCJ-02 fucking mar coordinates
        //to fit mongos' location indexing i have to use lower cases
        public string type { get; set; }
        public List<double> coordinates { get; set; }


        public void SetCoordinates(double longitude,double latitude)
        {
            coordinates = new List<double>();
            if (longitude > 180 || longitude < -180)
            {
                throw new System.Exception("wrong input");
            }
            if (latitude > 90 || latitude < -90)
            {
                throw new System.Exception("wrong input");
            }
            coordinates.Add(longitude);
            coordinates.Add(latitude);
        }

        public GeoPoint(double longitude, double latitude)
        {
            type = "Point";
            this.SetCoordinates(longitude, latitude);
        }

        public GeoPoint()
        {
            type = "Point";
            this.SetCoordinates(0, 0);
        }

    }
}
