using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.OldHouse.Business.Services;
using Jtext103.Volunteer.VolunteerEvent;
using Jtext103.Volunteer.VolunteerMessage;
using Jtext103.OldHouse.Business.Models;
using Jtext103.Repository;

namespace OldHouse.EventHandlers
{
    /// <summary>
    /// 用户Unlike老房子或者签到
    /// 删除已发feed
    /// </summary>
    public class UnlikeHandler : IVolunteerEventHandler
    {
        public string Name
        {
            get
            {
                return "UnlikeHandler";
            }
        }

        public void HandleEvent(Event oneEvent)
        {
            var service = HouseService.Instence;
            Guid userId = (Guid)oneEvent.Sender;
            var likeUser = service.MyUserManager.FindByIdAsync(userId).Result;
            Guid createEntityUserId;
            int houseOrCheckin = service.CheckTheIdIsHouseOrCheckin((Guid)oneEvent.Subject);
            string myContent;
            string title;
            QueryObject<Message> queryObject = new QueryObject<Message>(service.FeedService.EntityRepository);
            Dictionary<string, object> queryDic1 = new Dictionary<string, object>();
            Dictionary<string, object> queryDic2 = new Dictionary<string, object>();
            switch (houseOrCheckin)
            {
                case 0:
                    House house = service.FindOneById((Guid)oneEvent.Subject);
                    createEntityUserId = house.OwnerId;
                    //该用户给house创建者发的feed
                    title = "我赞了您发现的老房子";
                    myContent = "【" + likeUser.NickName + "(" + likeUser.UserName + ")】刚刚给你发现的【" + house.Name + "】点赞啦！"; 
                    queryDic2.Add("Title", title);
                    queryDic2.Add("Text", myContent);
                    queryDic2.Add("ReceiverId", house.OwnerId);
                    queryDic2.Add("MessageType", "Feed.TheEntityIsLikedByOther");
                    //删除找到的feed
                    queryObject.AppendQuery(queryDic1, QueryLogic.Or);
                    queryObject.AppendQuery(queryDic2, QueryLogic.Or);
                    //TODO BUG!!!胡斐然，，大爷的。。你把所有的feed都删了。。清空了
                    //service.FeedService.EntityRepository.Delete(queryObject);
                    break;
                case 1:
                    var checkIn = service.CheckInService.FindOneById((Guid)oneEvent.Subject);
                    House theHouse = service.FindOneById(checkIn.TargetId);
                    createEntityUserId = checkIn.User.Id;
                    //该用户给checkIn创建者发的feed
                    title = "我赞了您的签到";
                    myContent = "【" + likeUser.NickName + "(" + likeUser.UserName + ")】刚刚赞了你！";
                    queryDic2.Add("Title", title);
                    queryDic2.Add("Text", myContent);
                    queryDic2.Add("ReceiverId", checkIn.User.Id);
                    queryDic2.Add("MessageType", "Feed.TheEntityIsLikedByOther");
                    //删除找到的feed
                    queryObject.AppendQuery(queryDic1, QueryLogic.Or);
                    queryObject.AppendQuery(queryDic2, QueryLogic.Or);
                    //TODO BUG!!!胡斐然，，大爷的。。你把所有的feed都删了。。清空了
                    //service.FeedService.EntityRepository.Delete(queryObject);
                    break;
                default:
                    return;
            }
            //为house或者check in创建者扣5分
            var createEntityUser = service.MyUserManager.FindByIdAsync(createEntityUserId).Result;
            if (createEntityUser != null)
            {
                var profile = service.ProfileService.EntityRepository.FindOneById(createEntityUser.Profiles["OldHouseUserProfile"]);
                profile.MinusPoint(5);
                service.ProfileService.EntityRepository.SaveOne(profile);
            }

        }

        public List<Subscriber> GetHandlerSubscribers()
        {
            var subs = new List<Subscriber>();
            subs.Add(new Subscriber("UnlikeSubscriber", new List<string>() { "UnlikeEvent" }, new List<string>() { "UnlikeHandler" }, null));
            return subs;
        }
    }
}
