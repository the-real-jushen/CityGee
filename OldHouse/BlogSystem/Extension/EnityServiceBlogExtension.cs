using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.EntityModel;
using Jtext103.MongoDBProvider;
using Jtext103.Repository;
using MongoDB.Driver.Builders;
using System.Reflection;
using MongoDB.Driver;

namespace Jtext103.BlogSystem.Extension
{
    
    public static class EnityServiceBlogExtension
    {
        //for LRF, those method which will change the blog entity will be implemented here, for others pls use the LrfService
        //toggle fav
        /// <summary>
        /// toggole the favarate of the entity
        /// !!! Make sure you have the FavoriteTimes Field, ,it will update the LikeTimes in the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityService"></param>
        /// <param name="targetId"></param>
        /// <param name="userId"></param>
        /// <param name="lrfService"></param>
        /// <returns>the new isfavorite value</returns>
        [Obsolete("Do not use this, not tested and unfinished. use LrfService instead")]
        public static bool ToggleFavorite<T>(this EntityService<T> entityService, Guid targetId, Guid userId,
            LikeRateFavService lrfService) where T : Entity
        {
            var result = lrfService.ToggleFavorite(userId, targetId);
            if (result )
            {
                //fave times+1, make sure your entity has this field
                Dictionary<string, object> queryDict = new Dictionary<string, object>();
                queryDict.Add("_id", targetId);
                QueryObject < T > query = new QueryObject<T>(entityService.EntityRepository);
                query.AppendQuery(queryDict, QueryLogic.And);
                ((MongoDBRepository<T>)entityService.EntityRepository).Update(query,
                    Update.Inc("FavoriteTimes",1));
            }
            else
            {
                //fave times+1, make sure your entity has this field
                Dictionary<string, object> queryDict = new Dictionary<string, object>();
                queryDict.Add("_id", targetId);
                QueryObject<T> query = new QueryObject<T>(entityService.EntityRepository);
                query.AppendQuery(queryDict, QueryLogic.And);
                ((MongoDBRepository<T>)entityService.EntityRepository).Update(query,
                    Update.Inc("FavoriteTimes", -1));
            }
            return result;
        }

        //toggle like
        /// <summary>
        /// toggle the like of the entity, make sure the entity has the LikeTimes Field,it will update the LikeTimes in the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityService"></param>
        /// <param name="targetId"></param>
        /// <param name="userId"></param>
        /// <param name="lrfService"></param>
        /// <returns>the new isLike value</returns>
        [Obsolete("Do not use this, not tested and unfinished. use LrfService instead")]
        public static bool ToggleLike<T>(this EntityService<T> entityService, Guid targetId, Guid userId,
           LikeRateFavService lrfService) where T : Entity
        {
            var result = lrfService.ToggleLike(userId, targetId);
            if (result)
            {
                //fave times+1, make sure your entity has this field
                Dictionary<string, object> queryDict = new Dictionary<string, object>();
                queryDict.Add("_id", targetId);
                QueryObject<T> query = new QueryObject<T>(entityService.EntityRepository);
                query.AppendQuery(queryDict, QueryLogic.And);
                ((MongoDBRepository<T>)entityService.EntityRepository).Update(query,
                    Update.Inc("LikeTimes", 1));
            }
            else
            {
                //fave times+1, make sure your entity has this field
                Dictionary<string, object> queryDict = new Dictionary<string, object>();
                queryDict.Add("_id", targetId);
                QueryObject<T> query = new QueryObject<T>(entityService.EntityRepository);
                query.AppendQuery(queryDict, QueryLogic.And);
                ((MongoDBRepository<T>)entityService.EntityRepository).Update(query,
                    Update.Inc("LikeTimes", -1));
            }
            return result;
        }


        //rate
        /// <summary>
        /// rate the entity, make sure you have the RateTimes and Rating field in you entity, it will update the ratings in the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityService"></param>
        /// <param name="targetId"></param>
        /// <param name="userId"></param>
        /// <param name="lrfService"></param>
        /// <param name="rating"></param>
        /// <returns></returns>
        public static float Rate<T>(this EntityService<T> entityService, Guid targetId, Guid userId,
           LikeRateFavService lrfService, float rating) where T : Entity
        {
            var doIRate = lrfService.DoILikeRateFav(userId, targetId, LRFType.Rate);
            //calc new rating
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("_id", targetId);
            QueryObject<T> query = new QueryObject<T>(entityService.EntityRepository);
            query.AppendQuery(queryDict, QueryLogic.And);

            var entity = entityService.FindOneById(targetId);
            var oldRating=(float)entity.GetType().GetProperty("Rating").GetValue(entity, null);
            var oldRateTimes = lrfService.GetLRFCount(targetId, LRFType.Rate);
            if (!doIRate)
            {
                //my first time rating
                var newRating = (oldRating*oldRateTimes + rating)/(oldRateTimes + 1);
                lrfService.Rate(userId, targetId, rating);
                ((MongoDBRepository<T>)entityService.EntityRepository).Update(query,
                   Update.Set("Rating", newRating));
                return newRating;
            }
            else
            {
                //get my old rating
                var myOldRating = lrfService.GetMyRatingFor(userId, targetId);
                //calc the new one
                var newRating = (oldRating * oldRateTimes - myOldRating+rating) / (oldRateTimes );
                lrfService.Rate(userId, targetId, rating);
                ((MongoDBRepository<T>)entityService.EntityRepository).Update(query,
                   Update.Set("Rating", newRating));
                lrfService.Rate(userId, targetId, rating);
                return newRating;
            }    
            
            
        }

        //for now you can just use like service
    }
}
