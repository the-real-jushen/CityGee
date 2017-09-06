using System;
using System.Collections.Generic;
using System.Linq;
using Jtext103.EntityModel;
using Jtext103.Repository.Interface;
using Jtext103.Volunteer.Friend.Interfaces;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using Jtext103.MongoDBProvider;
using Jtext103.Repository;
using Jtext103.Volunteer.VolunteerEvent;

namespace Jtext103.Volunteer.Friend
{
    public class FollowService<T>:EntityService<T> where T:Entity,IFollowable
    {
        public FollowService(IRepository<T> repository ):base(repository)
        {
            
        }

        /// <summary>
        /// Am i following this user
        /// </summary>
        /// <param name="targetId">the followee profile id</param>
        /// <param name="followerId">can be a profile id</param>
        /// <returns></returns>
        public bool AmIFollowing(Guid targetId,Guid followerId)
        {
            return EntityRepository.FindOneById(followerId).FollowingIds.Contains(targetId);
        }

        /// <summary>
        /// let the followerId follow the target id
        /// </summary>
        /// <param name="targetId">target(followee) profile id</param>
        /// <param name="followerId">the follower Profile Id</param>
        public void Follow(Guid targetId, Guid followerId)
        {
            //不允许自己follow自己
            if (targetId == followerId)
            {
                throw new Exception("不允许自己follow自己");
            }
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("_id", followerId);
            QueryObject<T> query = new QueryObject<T>(EntityRepository);
            query.AppendQuery(queryDict, QueryLogic.And);
            ((MongoDBRepository<T>)EntityRepository).Update(query,
                   Update.AddToSet("FollowingIds", targetId));

            //increase the followee's follower count
            Dictionary<string, object> queryDict2 = new Dictionary<string, object>();
            queryDict2.Add("_id", targetId);
            QueryObject<T> query2 = new QueryObject<T>(EntityRepository);
            query2.AppendQuery(queryDict2, QueryLogic.And);

            ((MongoDBRepository<T>)EntityRepository).Update(query2,
                   Update.Inc("FollowerCount", 1));

            //产生用户start follow事件
            EventService.Publish("StartFollowingEvent", targetId, followerId);
        }

        /// <summary>
        /// stop following the target
        /// </summary>
        /// <param name="targetId">target(followee) profile id</param>
        /// <param name="followerId">the follower Profile Id</param>
        public void UnFollow(Guid targetId, Guid followerId)
        {
            //不允许自己unfollow自己
            if (targetId == followerId)
            {
                throw new Exception("不允许自己unfollow自己");
            }
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("_id", followerId);
            QueryObject<T> query = new QueryObject<T>(EntityRepository);
            query.AppendQuery(queryDict, QueryLogic.And);
            ((MongoDBRepository<T>)EntityRepository).Update(query,
                   Update.Pull("FollowingIds", targetId));

            //decrease the followee's follower count
            Dictionary<string, object> queryDict2 = new Dictionary<string, object>();
            queryDict2.Add("_id", targetId);
            QueryObject<T> query2 = new QueryObject<T>(EntityRepository);
            query2.AppendQuery(queryDict2, QueryLogic.And);

            ((MongoDBRepository<T>)EntityRepository).Update(query2,
                   Update.Inc("FollowerCount", -1));

            //产生用户stop follow事件
            EventService.Publish("StopFollowingEvent", targetId, followerId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="followerId"></param>
        /// <returns></returns>
        public bool ToggoleFollow(Guid targetId, Guid followerId)
        {
            if (AmIFollowing(targetId, followerId))
            {
                UnFollow(targetId, followerId);
                return false;
            }
            else
            {
                Follow(targetId, followerId);
                return true;
            }
        }

        /// <summary>
        /// get all followee id for a follower
        /// </summary>
        /// <param name="followerId"></param>
        /// <returns></returns>
        public IEnumerable<Guid> GetAllFollowingIds(Guid followerId)
        {
            return EntityRepository.FindOneById(followerId).FollowingIds;
        }

        /// <summary>
        /// 得到该用户被follow（关注）的次数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetMyFollowerCount(Guid id)
        {
            return EntityRepository.FindOneById(id).FollowerCount;
        }

    }
}
