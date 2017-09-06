using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Jtext103.BlogSystem;
using Jtext103.OldHouse.Business.Models;
using Jtext103.Volunteer.VolunteerMessage;
using Jtext103.Repository;

namespace Jtext103.OldHouse.Business.Services
{
    public static class ServiceExtention
    {
        /// <summary>
        /// 获得发送给指定用户id的feed
        /// 或者广播的feed
        /// </summary>
        /// <param name="feedService"></param>
        /// <param name="userId">用户id</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IEnumerable<Message> FindFeedFor(this MessageService feedService, Guid userId, int pageIndex, int pageSize)
        {
            QueryObject<Message> queryObject = new QueryObject<Message>(feedService.EntityRepository);
            //指定用户id
            Dictionary<string, object> queryDic1 = new Dictionary<string, object>();
            queryDic1.Add("ReceiverId", userId);
            //广播
            Dictionary<string, object> queryDic2 = new Dictionary<string, object>();
            queryDic2.Add("IsBroadcast", true);
            queryObject.AppendQuery(queryDic1,QueryLogic.Or);
            queryObject.AppendQuery(queryDic2,QueryLogic.Or);
            List<Message> myMessages = new List<Message>();
            myMessages = feedService.EntityRepository.Find(queryObject, "Time", false, pageIndex, pageSize).ToList();
            return myMessages;
        }
    }

}
