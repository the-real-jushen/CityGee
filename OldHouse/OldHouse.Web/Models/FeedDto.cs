using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jtext103.OldHouse.Business.Models;

namespace OldHouse.Web.Models
{
    public class FeedDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public List<string> Pictures { get; set; }
        public string DestinationLink { get; set; }
        public bool NewBlank { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public OldHouseUser MessageFrom { get; set; }
        public bool HasRead { get; set; }
        public bool HasDeleted { get; set; }
        public string Type { get; set; }
    }
}