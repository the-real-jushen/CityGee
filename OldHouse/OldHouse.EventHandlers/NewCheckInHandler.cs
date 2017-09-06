using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.OldHouse.Business.Services;
using Jtext103.Volunteer.VolunteerEvent;
using Jtext103.Volunteer.VolunteerMessage;
using Jtext103.OldHouse.Business.Models;
using Jtext103.BlogSystem;

namespace OldHouse.EventHandlers
{
    /// <summary>
    /// 添加了新的check in
    /// </summary>
    public class NewCheckInHandler : IVolunteerEventHandler
    {
        public List<Subscriber> GetHandlerSubscribers()
        {
            var subs = new List<Subscriber>();
            subs.Add(new Subscriber("NewCheckInSubscriber", new List<string>() { "NewCheckInEvent" }, new List<string>() { "NewCheckInHandler" }, null));
            return subs;
        }

        public void HandleEvent(Event oneEvent)
        {
            var service = HouseService.Instence;
            var checkIn = service.CheckInService.FindOneById((Guid)oneEvent.Subject);
            House theHouse = service.FindOneById(checkIn.TargetId);
            OldHouseUser user = service.MyUserManager.FindByIdAsync((Guid)oneEvent.Sender).Result;
            //string myContent = "您关注的用户 " + user.NickName + "(" + user.UserName + ")刚刚发表了关于老房子\"" + theHouse.Name + "\"的签到\"" + checkIn.Title + "\"！";
            string myContent = "【" + user.NickName + "(" + user.UserName + ")】到过了" + "【" + theHouse.Name + "】";
            string title = "我发表了新的签到";
            string link = "/house/Checkin/detail/" + checkIn.Id.ToString();
            List<string> picture = new List<string>();
            if (checkIn.Asset != null)
            {
                foreach (var asset in checkIn.Asset)
                {
                    if (asset.Type == Asset.IMAGE)
                    {
                        picture.Add(asset.Path);
                    }
                }
            }
            foreach (Guid receiverId in service.GetAllFollowerUserIds(user.Id))
            {
                var msg = new Message { Title = title, Text = myContent, IsBroadcast = false, MessageFrom = user.Id.ToString(), ReceiverId = receiverId, MessageType = "Feed.NewCheckIn", DestinationLink = link, Pictures = picture };
                service.FeedService.SendMessage(msg);
            }
            //为新check in创建者加10分
            var profile = service.ProfileService.EntityRepository.FindOneById(user.Profiles["OldHouseUserProfile"]);
            profile.AddPoint(10);
            service.ProfileService.EntityRepository.SaveOne(profile);
        }

        public string Name
        {
            get
            {
                return "NewCheckInHandler";
            }
        }
    }
}
