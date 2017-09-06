using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.OldHouse.Business.Services;
using Jtext103.Volunteer.VolunteerEvent;
using Jtext103.Volunteer.VolunteerMessage;
using Jtext103.OldHouse.Business.Models;

namespace OldHouse.EventHandlers
{
    /// <summary>
    /// 用户Like某用户创建的老房子或者签到
    /// </summary>
    public class TheEntityIsLikedByOtherHandler : IVolunteerEventHandler
    {
        public string Name
        {
            get 
            {
                return "TheEntityIsLikedByOtherHandler";
            }
        }

        public void HandleEvent(Event oneEvent)
        {
            var service = HouseService.Instence;
            var likeUser = service.MyUserManager.FindByIdAsync((Guid)oneEvent.Sender).Result;
            Guid createEntityUserId;
            int houseOrCheckin = service.CheckTheIdIsHouseOrCheckin((Guid)oneEvent.Subject);
            string myContent;
            string title;
            Message msg;
            string link = "/Profile/HomePage/" + likeUser.Id.ToString();
            switch (houseOrCheckin)
            {
                case 0:
                    House house = service.FindOneById((Guid)oneEvent.Subject);
                    title = "我赞了您发现的老房子";
                    //myContent = "用户 " + likeUser.NickName + "(" + likeUser.UserName + ")刚刚赞了您发现的老房子\"" + house.Name + "\"！";
                    myContent = "【" + likeUser.NickName + "(" + likeUser.UserName + ")】刚刚给你发现的【" + house.Name + "】点赞啦！";
                    createEntityUserId = house.OwnerId;
                    break;
                case 1:
                    var checkIn = service.CheckInService.FindOneById((Guid)oneEvent.Subject);
                    House theHouse = service.FindOneById(checkIn.TargetId);
                    title = "我赞了您的签到";
                    //myContent = "用户 " + likeUser.NickName + "(" + likeUser.UserName + ")刚刚赞了您写的关于老房子\"" + theHouse.Name + "\"的签到\"" + checkIn.Title + "\"！";
                    myContent = "【" + likeUser.NickName + "(" + likeUser.UserName + ")】刚刚赞了你！";
                    createEntityUserId = checkIn.User.Id;
                    break;
                default:
                    return;
            }
            msg = new Message { Title = title, Text = myContent, IsBroadcast = false, MessageFrom = likeUser.Id.ToString(), ReceiverId = createEntityUserId, MessageType = "Feed.TheEntityIsLikedByOther", DestinationLink = link };
            service.FeedService.SendMessage(msg);

            //为house或者check in创建者加5分
            var createEntityUser = service.MyUserManager.FindByIdAsync(createEntityUserId).Result;
            if (createEntityUser != null)
            {
                var profile = service.ProfileService.EntityRepository.FindOneById(createEntityUser.Profiles["OldHouseUserProfile"]);
                profile.AddPoint(5);
                service.ProfileService.EntityRepository.SaveOne(profile);
            }
        }

        public List<Subscriber> GetHandlerSubscribers()
        {
            var subs = new List<Subscriber>();
            subs.Add(new Subscriber("TheEntityIsLikedByOtherSubscriber", new List<string>() { "LikeEvent" }, new List<string>() { "TheEntityIsLikedByOtherHandler" }, null));
            return subs;
        }
    }
}
