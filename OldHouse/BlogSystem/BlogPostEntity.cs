using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.BlogSystem.Interface;

namespace Jtext103.BlogSystem
{
    public class BlogPostEntity : BlogEntity
    {
        public new const string  TYPE = "BlogPost";

        public string BlogType { get; set; }
        //标题
        public string Title { get; set; }
        //是否已激活
        public bool IsActivated { get; set; }
        //修改时间
        public DateTime ModifyTime { get; set; }
        public List<Asset> Asset { get; set; }
        //阅读次数
       

        public BlogPostEntity():base()
        {
                
        }
        
        public BlogPostEntity(BasicUser user, Guid targetId, string title, string content)
            : base(user, targetId, content)
        {
            Title = title;
            IsActivated = false;
            ModifyTime = DateTime.Now;
            this.Asset = new List<Asset>();
            EntityType = TYPE;
            BlogType = TYPE;
        }



        public BlogPostEntity(BasicUser user, Guid targetId, string title, string content,List<Asset> asset)
            : base(user, targetId, content)
        {
            Title = title;
            IsActivated = false;
            ModifyTime = DateTime.Now;
            this.Asset = new List<Asset>();
            ReadTimes = 0;
            LikedTimes = 0;
            ReprientedTimes = 0;
            Rating = 0;
            FavoriteTimes = 0;
            EntityType = TYPE;
            BlogType = TYPE;
            Asset = asset;
        }



    }
}
