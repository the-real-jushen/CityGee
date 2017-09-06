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
        public BlogEntity(BasicUser user, Guid targetId, string content):base()
        {
            EntityType = TYPE;
            
            User = user;
            TargetId = targetId;
            Content = content;
            CreateTime = DateTime.Now;
            HasDeleted = false;
        }

        public BlogEntity()
        {
            EntityType = TYPE;
            User = new BasicUser();
            CreateTime = DateTime.Now;
            HasDeleted = false;
        }



        public int LikedTimes
        {
            get;
            set;
        }
    }
}
