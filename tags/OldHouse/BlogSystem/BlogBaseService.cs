using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.EntityModel;
using Jtext103.Repository.Interface;
using Jtext103.Repository;

namespace Jtext103.BlogSystem
{
    /// <summary>
    /// provide common functions for comment and blogpost
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlogBaseService<T>: EntityService<T> where T: BlogEntity
    {
        private LikeRateService _likeServiver;
        public BlogBaseService(IRepository<T> repository, LikeRateService likeService)
            : base(repository)
        {
            _likeServiver = likeService;
        }

        /// <summary>
        /// like a blog post or a comment, if you already liked it you can not do it again
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <returns>if the has seuccess</returns>
        public virtual bool LikeBlog( Guid userId,Guid targetId)
        {
            if (_likeServiver.DoIRated(userId, targetId))
            {
                return false;
            }
            else
            {
                _likeServiver.Rate(userId, targetId, 1);
                var blog =FindOneById(targetId);
                //this is not thread safe, !!!!! revise it
                blog.LikedTimes =_likeServiver.GetRatedCount(targetId);
                this.SaveOne(blog);
                return true;
            }
        }

        /// <summary>
        /// unlike a blog or comment if you have previously liked
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <returns>if it is successful</returns>
        public virtual bool UnLikeBlog(Guid userId, Guid targetId)
        {
            if (_likeServiver.DoIRated(userId, targetId))
            {
                _likeServiver.ClearMyRate(userId, targetId);
                var blog = FindOneById(targetId);
                //this is not thread safe, !!!!! revise it
                blog.LikedTimes = _likeServiver.GetRatedCount(targetId);
                this.SaveOne(blog);
                return true;
            }
            else
            {
               return false;
            }
        }
    }
}
