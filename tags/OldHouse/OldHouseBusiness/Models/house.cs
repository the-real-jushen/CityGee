using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.EntityModel;

namespace Jtext103.OldHouse.Business.Models
{
    /// <summary>
    /// holds the info of a house
    /// </summary>
    public class House:Entity
    {
        /// <summary>
        /// the user guid whom added by
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// need register
        /// </summary>
        private double _overallValue;
        public string Name { get; set; }

        public string LocationString { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }

        public string City { get; set; }

        public string Abstarct { get; set; }

        public string Description { get; set; }

        public string Condition { get; set; }
        public DateTime BuiltYear { get; set; }

        public double HistoricValue{ get; set; }
        public double PhotoValue { get; set; }


        public List<string> Images { get; set; }
        public List<string> Tags { get; set; }
        public string Cover { get; set; }


        /// <summary>
        /// a unique code name for a house, unlike the ID its's human readable
        /// /// </summary>
        public string CodeName { get; set; }
        public double OverallValue
        {
            get
            {
                return _overallValue;
            }
            internal set
            {
                _overallValue = value;
            }
        }

        public GeoPoint Location { get; set; }



        
        
        
        public House():base()
        {
            this.EntityType = "House";
        }
    }
}
