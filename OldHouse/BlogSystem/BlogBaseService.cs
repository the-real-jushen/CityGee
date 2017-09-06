using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.EntityModel;
using Jtext103.Repository.Interface;
using Jtext103.Repository;
//fuck i use the mongodb directly, to do update to be specific

namespace Jtext103.BlogSystem
{
    /// <summary>
    /// provide common functions for comment and blogpost
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlogBaseService<T>: EntityService<T> where T: BlogEntity
    {
        public LikeRateFavService LrfService;
        public BlogBaseService(IRepository<T> repository, LikeRateFavService likeService)
            : base(repository)
        {
            LrfService = likeService;
        }



        ///// <summary>
        ///// like a blog post or a comment, if you already liked it you can not do it again
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="targetId"></param>
        ///// <returns>if the has seuccess</returns>
        //public virtual bool LikeBlog( Guid userId,Guid targetId)
        //{
        //    if (LikeServiver.DoILikeRateFav(userId, targetId))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        LikeServiver.Rate(userId, targetId, 1);
        //        var blog =FindOneById(targetId);
        //        //this is not thread safe, !!!!! revise it
        //        blog.LikedTimes =LikeServiver.GetRatedCount(targetId);
        //        this.SaveOne(blog);
        //        return true;
        //    }
        //}

        ///// <summary>
        ///// unlike a blog or comment if you have previously liked
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="targetId"></param>
        ///// <returns>if it is successful</returns>
        //public virtual bool UnLikeBlog(Guid userId, Guid targetId)
        //{
        //    if (LikeServiver.DoILikeRateFav(userId, targetId))
        //    {
        //        LikeServiver.ClearMyRate(userId, targetId);
        //        var blog = FindOneById(targetId);
        //        //this is not thread safe, !!!!! revise it
        //        blog.LikedTimes = LikeServiver.GetRatedCount(targetId);
        //        this.SaveOne(blog);
        //        return true;
        //    }
        //    else
        //    {
        //       return false;
        //    }
        //}
    }
}
