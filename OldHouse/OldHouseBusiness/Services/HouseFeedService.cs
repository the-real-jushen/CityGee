using Jtext103.Repository;
using Jtext103.Volunteer.VolunteerMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.OldHouse.Business.Services
{
    public partial class HouseService
    {
        /// <summary>
        /// 获得所有发给该用户的feed
        /// （不包括广播，除了该用户自己发的）
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="sortByKey"></param>
        /// <param name="isAscending"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Message> FindMyFeeds(Guid receiverId, string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
            QueryObject<Message> queryObject = new QueryObject<Message>(FeedService.EntityRepository);
            Dictionary<string, object> queryDic = new Dictionary<string, object>();
            queryDic.Add("ReceiverId", receiverId);
            Dictionary<string, object> subQueryDic = new Dictionary<string, object>();
            subQueryDic.Add("$ne", receiverId.ToString());
            queryDic.Add("MessageFrom", subQueryDic);
            queryObject.AppendQuery(queryDic, QueryLogic.And);
            List<Message> myMessages = new List<Message>();
            myMessages = FeedService.EntityRepository.Find(queryObject, sortByKey, isAscending, pageIndex, pageSize).ToList();
            return myMessages;
        }

        /// <summary>
        /// 获得所有发给该用户的feed的个数
        /// （不包括广播，除了该用户自己发的）
        /// </summary>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        public long FindMyFeedCount(Guid receiverId)
        {
            QueryObject<Message> queryObject = new QueryObject<Message>(FeedService.EntityRepository);
            Dictionary<string, object> queryDic = new Dictionary<string, object>();
            queryDic.Add("ReceiverId", receiverId);
            Dictionary<string, object> subQueryDic = new Dictionary<string, object>();
            subQueryDic.Add("$ne", receiverId.ToString());
            queryDic.Add("MessageFrom", subQueryDic);
            queryObject.AppendQuery(queryDic, QueryLogic.And);
            long result = FeedService.EntityRepository.FindCountOfResult(queryObject);
            return result;
        }

        /// <summary>
        /// 获得所有广播及发给该用户的feed的个数
        /// （除了该用户自己发的）
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="sortByKey"></param>
        /// <param name="isAscending"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Message> FindBroadcastAndMyFeeds(Guid receiverId, string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
            QueryObject<Message> queryObject = new QueryObject<Message>(FeedService.EntityRepository);
            //发给自己的
            Dictionary<string, object> queryDic1 = new Dictionary<string, object>();
            queryDic1.Add("ReceiverId", receiverId);
            Dictionary<string, object> subQueryDic1 = new Dictionary<string, object>();
            subQueryDic1.Add("$ne", receiverId.ToString());
            queryDic1.Add("MessageFrom", subQueryDic1);
            queryObject.AppendQuery(queryDic1, QueryLogic.And);
            //广播
            Dictionary<string, object> queryDic2 = new Dictionary<string, object>();
            queryDic2.Add("IsBroadcast", true);
            Dictionary<string, object> subQueryDic2 = new Dictionary<string, object>();
            subQueryDic2.Add("$ne", receiverId.ToString());
            queryDic2.Add("MessageFrom", subQueryDic2);
            queryObject.AppendQuery(queryDic2, QueryLogic.Or);

            List<Message> myMessages = new List<Message>();
            myMessages = FeedService.EntityRepository.Find(queryObject, sortByKey, isAscending, pageIndex, pageSize).ToList();
            return myMessages;
        }

        /// <summary>
        /// 获得所有广播及发给该用户的feed的个数
        /// （除了该用户自己发的）
        /// </summary>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        public long FindBroadcastAndMyFeedCount(Guid receiverId)
        {
            QueryObject<Message> queryObject = new QueryObject<Message>(FeedService.EntityRepository);
            //发给自己的
            Dictionary<string, object> queryDic1 = new Dictionary<string, object>();
            queryDic1.Add("ReceiverId", receiverId);
            Dictionary<string, object> subQueryDic1 = new Dictionary<string, object>();
            subQueryDic1.Add("$ne", receiverId.ToString());
            queryDic1.Add("MessageFrom", subQueryDic1);
            queryObject.AppendQuery(queryDic1, QueryLogic.And);
            //广播
            Dictionary<string, object> queryDic2 = new Dictionary<string, object>();
            queryDic2.Add("IsBroadcast", true);
            Dictionary<string, object> subQueryDic2 = new Dictionary<string, object>();
            subQueryDic2.Add("$ne", receiverId.ToString());
            queryDic2.Add("MessageFrom", subQueryDic2);
            queryObject.AppendQuery(queryDic2, QueryLogic.Or);

            long result = FeedService.EntityRepository.FindCountOfResult(queryObject);
            return result;
        }

        /// <summary>
        /// 获得所有广播feed
        /// （除了自己发出的）
        /// <param name="receiverId">接收feed的人</param>
        /// <param name="sortByKey"></param>
        /// <param name="isAscending"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Message> FindBroadcastFeeds(Guid receiverId, string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
            QueryObject<Message> queryObject = new QueryObject<Message>(FeedService.EntityRepository);
            Dictionary<string, object> queryDic = new Dictionary<string, object>();
            queryDic.Add("IsBroadcast", true);
            Dictionary<string, object> subQueryDic = new Dictionary<string, object>();
            subQueryDic.Add("$ne", receiverId.ToString());
            queryDic.Add("MessageFrom", subQueryDic);
            queryObject.AppendQuery(queryDic, QueryLogic.And);
            List<Message> myMessages = new List<Message>();
            myMessages = FeedService.EntityRepository.Find(queryObject, sortByKey, isAscending, pageIndex, pageSize).ToList();
            return myMessages;
        }

        /// <summary>
        /// 获得所有广播feed的个数
        /// （除了自己发出的）
        /// </summary>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        public long FindBroadcastFeedCount(Guid receiverId)
        {
            QueryObject<Message> queryObject = new QueryObject<Message>(FeedService.EntityRepository);
            Dictionary<string, object> queryDic = new Dictionary<string, object>();
            queryDic.Add("IsBroadcast", true);
            Dictionary<string, object> subQueryDic = new Dictionary<string, object>();
            subQueryDic.Add("$ne", receiverId.ToString());
            queryDic.Add("MessageFrom", subQueryDic);
            queryObject.AppendQuery(queryDic, QueryLogic.And);
            long result = FeedService.EntityRepository.FindCountOfResult(queryObject);
            return result;
        }
    }
}
