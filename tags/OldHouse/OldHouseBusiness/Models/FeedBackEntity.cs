using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.EntityModel;

namespace Jtext103.OldHouse.Business.Models
{
    public class FeedBackEntity:Entity
    {
        public string CreatedUrl { get; set; }
        
        public string UserName { get; set; }

        public string Content { get; set; }
    }
}
