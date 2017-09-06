using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.BlogSystem;

namespace Jtext103.OldHouse.Business.Models
{
    public class CheckIn:BlogPostEntity
    {
        public GeoPoint Location { get; set; }

        public bool IsEssence { get; set; }

        public CheckIn():base()
        {
            IsEssence = false;
        }
        
        public CheckIn(BasicUser user, Guid targetId, string title, string content,List<Asset> asset,GeoPoint location ):base(user,targetId,title,content,asset)
        {
            Location = location;
            EntityType = "CheckIn";
            BlogType = "CheckIn";
            IsEssence = false;
        }
    }
}
