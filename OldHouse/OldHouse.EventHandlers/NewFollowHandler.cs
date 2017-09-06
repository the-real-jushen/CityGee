using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.OldHouse.Business.Services;
using Jtext103.Volunteer.VolunteerEvent;
using Jtext103.Volunteer.VolunteerMessage;

namespace OldHouse.EventHandlers
{
    /// <summary>
    /// 有新的关注
    /// </summary>
    public class NewFollowHandler : IVolunteerEventHandler
    {
        public List<Subscriber> GetHandlerSubscribers()
        {
            var subs = new List<Subscriber>();
            subs.Add(new Subscriber("NewFollowerSubscriber", new List<string>() { "StartFollowingEvent" }, new List<string>() { "NewFollowerHandler" }, null));
            return subs;
        }

        public void HandleEvent(Event oneEvent)
        {
            var service = HouseService.Instence;
            var followerUid = service.FollowService.FindOneById((Guid)(oneEvent.Sender)).UserId;
            var followeeUid = service.FollowService.FindOneById((Guid)(oneEvent.Subject)).UserId;
            var followerUser = service.MyUserManager.FindByIdAsync(followerUid).Result;
            var followeeUser = service.MyUserManager.FindByIdAsync(followeeUid).Result;
            //string myContent = "用户 " + followerUser.NickName + "(" + followerUser.UserName + ") 刚刚关注了您！";
            string myContent = "有新朋友关注你：【" + followerUser.NickName + "(" + followerUser.UserName + ")】";
            string title = "新关注";
            string link = "/Profile/HomePage/" + followerUid.ToString();
            
            //todo add profile link to the follower, send out the msg, add the type
            //todo fix the hard code
            var msg = new Message { Title = title, Text = myContent, IsBroadcast = false, MessageFrom = followerUid.ToString(), ReceiverId = followeeUid, MessageType = "Feed.NewFollower", DestinationLink = link };
            service.FeedService.SendMessage(msg);

            //为followee加5分
            var profile = service.ProfileService.EntityRepository.FindOneById(followeeUser.Profiles["OldHouseUserProfile"]);
            profile.AddPoint(5);
            service.ProfileService.EntityRepository.SaveOne(profile);
        }

        public string Name
        {
            get
            {
                return "NewFollowerHandler";
            }
        }
    }
}
