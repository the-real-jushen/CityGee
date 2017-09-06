using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.EntityModel;
using Jtext103.Repository.Interface;
using Jtext103.Repository;
using Jtext103.Volunteer.VolunteerEvent;

namespace Jtext103.BlogSystem
{
    /// <summary>
    /// use this to handle favorate like ratings, user can rate multiple times but it is very un-recommanded
    /// </summary>
    public class LikeRateFavService:EntityService<LikeRateFavEntity>
    {
        public LikeRateFavService(IRepository<LikeRateFavEntity> repository):base(repository)
        {
            
        }

        /// <summary>
        /// return if i like rated faved a target
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public virtual bool DoILikeRateFav(Guid ownerId, Guid targetId,LRFType type)
        {
            Dictionary<string,object> queryDict=new Dictionary<string,object>();
            queryDict.Add("OwnerId",ownerId);
            queryDict.Add("TargetId",targetId);
            queryDict.Add("Type", type);
            var query=getNewQueryObject();
            query.AppendQuery(queryDict,QueryLogic.And);
            return EntityRepository.Find(query).Any();
        }

        /// <summary>
        /// get my rating for a target, will throw if you have not rated,if you rated more than onec this will return the first one
        /// ,only work for rating, please do not rate more than once!!
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public virtual float GetMyRatingFor(Guid userId, Guid targetId)
        {

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("OwnerId", userId);
            queryDict.Add("TargetId", targetId);
            queryDict.Add("Type", LRFType.Rate);
            var query = getNewQueryObject();
            query.AppendQuery(queryDict, QueryLogic.And);
            var rating=EntityRepository.Find(query).FirstOrDefault();
            if (rating != null)
            {
                return rating.Rating;
            }
            else
            {
                throw new System.Exception("Not Found");
            }
        }

        /// <summary>
        /// get many time is a target been likeed/rated/faved
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual int GetLRFCount(Guid targetId,LRFType type)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("TargetId", targetId);
            queryDict.Add("Type", type);
            var query = getNewQueryObject();
            query.AppendQuery(queryDict, QueryLogic.And);
            return EntityRepository.Find(query).Count();
        }

        /// <summary>
        /// get all the LRF entities for a target with the specified type
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual IEnumerable<LikeRateFavEntity> GetAllLrfFor(Guid targetId,LRFType type)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("TargetId", targetId);
            queryDict.Add("Type", type);
            var query = getNewQueryObject();
            query.AppendQuery(queryDict, QueryLogic.And);
            return EntityRepository.Find(query);
        }

        /// <summary>
        /// get all target ids that LRF by the user,
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual IEnumerable<Guid> GetAllMyLRFIds(Guid ownerId, LRFType type)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("OwnerId", ownerId);
            queryDict.Add("Type", type);
            var query = getNewQueryObject();
            query.AppendQuery(queryDict, QueryLogic.And);
            return EntityRepository.Find(query).Select(e=>e.TargetId);
        }

        
        /// <summary>
        /// clear all my ratings, fav, like,
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        public virtual void ClearMyLRF(Guid userId, Guid targetId,LRFType type)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("OwnerId", userId);
            queryDict.Add("TargetId", targetId);
            queryDict.Add("Type", type);
            var query = getNewQueryObject();
            query.AppendQuery(queryDict, QueryLogic.And);
            EntityRepository.Delete(query);
        }

        /// <summary>
        /// rate a stuff
        /// </summary>
        /// <param name="userId">user rated it</param>
        /// <param name="targetId">the stuff </param>
        /// <param name="rating"></param>
        /// <param name="allowedMultiRate"></param>
        /// <returns></returns>
        public void Rate(Guid userId, Guid targetId, float rating)
        {
            if (DoILikeRateFav(userId, targetId, LRFType.Rate))
            {
                Dictionary<string, object> queryDict = new Dictionary<string, object>();
                queryDict.Add("OwnerId", userId);
                queryDict.Add("TargetId", targetId);
                queryDict.Add("Type", LRFType.Rate);
                EntityRepository.Update(queryDict,new Dictionary<string, object>{{"Rating",rating}});
            }
            else
            {
                EntityRepository.SaveOne(
                    new LikeRateFavEntity
                    {
                        OwnerId = userId,
                        TargetId = targetId,
                        Rating = rating,
                        Type = LRFType.Rate
                    });
            }
        }


        /// <summary>
        /// set user read a entity
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        public void Read(Guid userId, Guid targetId)
        {
            if (DoILikeRateFav(userId, targetId, LRFType.Read))
            {
                //already read
                return;
            }
            else
            {
                EntityRepository.SaveOne(
                    new LikeRateFavEntity
                    {
                        OwnerId = userId,
                        TargetId = targetId,
                        Type = LRFType.Read
                    });
            }
        }

        /// <summary>
        /// toggle favorate or a target
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <returns>the new state for this target</returns>
        public bool ToggleFavorite(Guid userId, Guid targetId)
        {
            return ToggleLikeFav(userId, targetId, LRFType.Favorate);
        }

        /// <summary>
        /// toggle favorate or a target
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <returns>the new state for this target</returns>
        public bool ToggleLike(Guid userId, Guid targetId)
        {
            bool result = ToggleLikeFav(userId, targetId, LRFType.Like);
            if (result == true)
            {
                //赞
                //产生用户like事件
                EventService.Publish("LikeEvent", targetId, userId);
            }
            else
            {
                //取消赞
                //产生用户unlike事件
                EventService.Publish("UnlikeEvent", targetId, userId);
            }
            return result;
        }

        /// <summary>
        /// Toggle a Favorite or Like of a target
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <param name="type">olny accepte like or favorite</param>
        /// <returns>the new state for this target</returns>
        public bool ToggleLikeFav(Guid userId, Guid targetId, LRFType type)
        {
            if (type != LRFType.Like && type != LRFType.Favorate)
            {
                throw new Exception("olny accepte like or favorite");
            }
            if (DoILikeRateFav(userId, targetId, type))
            {
                ClearMyLRF(userId, targetId, type);
                return false;
            }
            else
            {
                EntityRepository.SaveOne(
                    new LikeRateFavEntity
                    {
                        OwnerId = userId,
                        TargetId = targetId,
                        Type = type
                    });
                return true;
            }
        }


    }
}
