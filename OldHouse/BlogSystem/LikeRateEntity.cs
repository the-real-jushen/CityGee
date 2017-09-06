using Jtext103.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.BlogSystem
{
    /// <summary>
    /// 
    /// </summary>
    public class LikeRateFavEntity:Entity
    {

        public LRFType Type { get; set; }

        //only available when type is rate
        public float Rating { get; set; }

        public Guid TargetId{ get; set; }

        public Guid OwnerId { get; set; }
        public LikeRateFavEntity():base()
        {
            //like is the defualt
            Type=LRFType.Like;
        }
    }

    public enum LRFType
    {
        None,
        Like,
        Rate,
        Read,
        Favorate
    }
}
