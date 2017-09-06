using Jtext103.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.OldHouse.Business.Models
{
    public class Article:Entity
    {
        public Guid AuthorId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public Article()
        {
            CreatedTime = DateTime.Now;
        }
    }
}
