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
    public class BlogPostService:BlogBaseService<BlogPostEntity>
    {
       
        public BlogPostService(IRepository<BlogPostEntity> repository,LikeRateFavService likeService )
            : base(repository,likeService)
        {
            
            //注册BlogPost的私有属性
            //EntityRepository.RegisterMap<BlogPostEntity>(new List<string>() { "readTimes", "likedTimes", "reprintedTimes", "rating" });
        }

        

        public virtual void AddBlogPost(BasicUser user, Guid targetId, string title, string content)
        {
            BlogPostEntity blogPostEntity = new BlogPostEntity(user, targetId, title, content);
            EntityRepository.SaveOne(blogPostEntity);
        }

        public virtual void AddBlogPost(BasicUser user, Guid targetId, string title, string content,List<Asset> asset)
        {
            BlogPostEntity blogPostEntity = new BlogPostEntity(user, targetId, title, content,asset);
            EntityRepository.SaveOne(blogPostEntity);
        }

        /// <summary>
        /// 将blogPost标记为已删除
        /// </summary>
        public void DeleteBlogPost(Guid blogPostId)
        {
            BlogPostEntity blogPostEntity = FindOneById(blogPostId);
            blogPostEntity.HasDeleted = true;
            EntityRepository.SaveOne(blogPostEntity);
        }

        /// <summary>
        /// 激活blogPost
        /// </summary>
        public virtual void ActivateBlogPost(BlogPostEntity blogPostEntity)
        {
            if (blogPostEntity.IsActivated == false)
            {
                blogPostEntity.IsActivated = true;
                blogPostEntity.ModifyTime = DateTime.Now;
                EntityRepository.SaveOne(blogPostEntity);
            }
        }

        /// <summary>
        /// 修改blogPost
        /// </summary>
        public virtual void ModifyBlogPost(BlogPostEntity blogPostEntity, string title, string content)
        {
            blogPostEntity.Title = title;
            blogPostEntity.Content = content;
            blogPostEntity.ModifyTime = DateTime.Now;
            EntityRepository.SaveOne(blogPostEntity);
        }

        public virtual void ModifyBlogPost(BlogPostEntity blogPostEntity, string title, string content, List<Asset> asset)
        {
            blogPostEntity.Title = title;
            blogPostEntity.Content = content;
            blogPostEntity.ModifyTime = DateTime.Now;
            blogPostEntity.Asset = asset;
            EntityRepository.SaveOne(blogPostEntity);
        }



        /// <summary>
        /// 找到所有对targetId的BlogPost的个数
        /// </summary>
        public long FindAllBlogPostCountFor(Guid targetId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("TargetId", targetId);
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            long result = EntityRepository.FindCountOfResult(queryObject);
            return result;
        }

        /// <summary>
        /// 找到所有对targetId的BlogPost
        /// </summary>
        public virtual IEnumerable<BlogPostEntity> FindAllBlogPostFor(Guid targetId, string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("TargetId", targetId);
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            IEnumerable<BlogPostEntity> result = EntityRepository.Find(queryObject, sortByKey, isAscending, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// find all blogs (checkins) made by a user, this will have the returned checins in oder of target house id, so kinda grouped
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IEnumerable<BlogPostEntity> FindAllBlogPostForUser(Guid userId,  int pageIndex, int pageSize)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("User._id", userId);
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            IEnumerable<BlogPostEntity> result = EntityRepository.Find(queryObject, "CreateTime", false, pageIndex, pageSize);
            return result;
        }

        public long FindAllBlogPostCountForUser(Guid userId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("User._id", userId);
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            long result = EntityRepository.FindCountOfResult(queryObject);
            return result;
        }

        /// <summary>
        /// 找到该用户Like过的所有BlogPostEntity
        /// </summary>
        /// <param name="userId">该用户id</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<BlogPostEntity> FindLikedBlogPostByUser(Guid userId,  int pageIndex, int pageSize)
        {
            var likedHouseIds = LrfService.GetAllMyLRFIds(userId, LRFType.Like);
            var query = this.getNewQueryObject();
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            Dictionary<string, object> subQueryDict = new Dictionary<string, object>();
            subQueryDict.Add("$in", likedHouseIds);
            queryDict.Add("_id", subQueryDict);
            query.AppendQuery(queryDict, QueryLogic.And);
            return EntityRepository.Find(query, pageIndex, pageSize);
        }
        /// <summary>
        /// 找到该用户Like过的所有BlogPostEntity的数目
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public long FindLikedBlogPostCountByUser(Guid userId)
        {
            var likedHouseIds = LrfService.GetAllMyLRFIds(userId, LRFType.Like);
            var query = this.getNewQueryObject();
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            Dictionary<string, object> subQueryDict = new Dictionary<string, object>();
            subQueryDict.Add("$in", likedHouseIds);
            queryDict.Add("_id", subQueryDict);
            query.AppendQuery(queryDict, QueryLogic.And);
            return EntityRepository.FindCountOfResult(query);
        }

    }
}
