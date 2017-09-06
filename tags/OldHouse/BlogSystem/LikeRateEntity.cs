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
    public class LikeRateEntity:Entity
    {
        
        public List<Rating> Ratings { get; set; }
        public LikeRateEntity():base()
        {

        }
    }

    public class Rating
    {
        public Guid RatedBy { get; set; }
        public double  MyRating { get; set; }
    }
}
