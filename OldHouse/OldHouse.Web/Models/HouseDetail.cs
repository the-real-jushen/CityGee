using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    public class HouseDetail:HouseBrief
    {
        public string BuiltYear { get; set; }
        public string Description { get; set; }

        public List<string> Images { get; set; }

        public List<string> Tags { get; set; }

        public Guid OwnerId { get; set; }

        public string OwnerName { get; set; }
        public string OwnerAvatar { get; set; }
    }
}