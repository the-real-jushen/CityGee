using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.EntityModel;
using Jtext103.BlogSystem.Interface;

namespace Jtext103.BlogSystem
{
    public class BlogEntity:Entity,ILikeable
    {
        public const string TYPE = "Blog";

        //发表人
        public BasicUser User { get; set; }
        //目标id
        public Guid TargetId { get; set; }
        //内容
        public string Content { get; set; }
        //创建时间
        public DateTime CreateTime { get; set; }
        //评论是否被删除
        public bool HasDeleted { get; set; }


        [Obsolete("do not use this, unfinished, use LRF service")]
        public int ReadTimes { get; set; }
        //点赞次数:base

        [Obsolete("do not use this, unfinished, use LRF service")]
        public int FavoriteTimes { get; set; }

        [Obsolete("do not use this, unfinished, use LRF service")]
        public int RateTimes { get; set; }
        
        [Obsolete("do not use this, unfinished, use LRF service")]
        public int LikedTimes
        {
            get;
            set;
        }


        //转载次数
        public int ReprientedTimes { get; set; }
        //得分
        public double Rating { get; set; }



        public BlogEntity(BasicUser user, Guid targetId, string content):base()
        {
            EntityType = TYPE;
            User = user;
            TargetId = targetId;
            Content = content;
            CreateTime = DateTime.Now;
            HasDeleted = false;
            //ReadTimes = 0;
            //LikedTimes = 0;
            ReprientedTimes = 0;
            Rating = 0;
            //FavoriteTimes = 0;
        }

        public BlogEntity()
        {
            EntityType = TYPE;
            User = new BasicUser();
            CreateTime = DateTime.Now;
            HasDeleted = false;
            //ReadTimes = 0;
            //LikedTimes = 0;
            ReprientedTimes = 0;
            Rating = 0;
            //FavoriteTimes = 0;
        }



        
    }
}
