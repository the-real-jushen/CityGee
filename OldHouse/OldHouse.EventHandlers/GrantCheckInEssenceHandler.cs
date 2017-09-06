using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.OldHouse.Business.Models;
using Jtext103.OldHouse.Business.Services;
using Jtext103.Volunteer.VolunteerEvent;
using Jtext103.Volunteer.VolunteerMessage;
using Jtext103.BlogSystem;

namespace OldHouse.EventHandlers
{
    /// <summary>
    /// check in被管理员设为精华
    /// </summary>
    public class GrantCheckInEssenceHandler : IVolunteerEventHandler
    {
        public string Name
        {
            get 
            {
                return "GrantCheckInEssenceHandler";
            }
        }

        public void HandleEvent(Event oneEvent)
        {
            var service = HouseService.Instence;
            var checkIn = service.CheckInService.FindOneById((Guid)oneEvent.Subject);
            Guid userId = checkIn.User.Id;
            string title = "新的精华";
            string myContent = "您发表的【" + checkIn.Title + "】刚刚被设为精华！";
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
            Message msg = new Message { Title = title, Text = myContent, IsBroadcast = false, MessageFrom = "system", ReceiverId = userId, MessageType = "Feed.GrantCheckInEssence", DestinationLink = link, Pictures = picture };
            service.FeedService.SendMessage(msg);

            //为该用户加50分
            var user = service.MyUserManager.FindByIdAsync(userId).Result;
            if (user != null)
            {
                var profile = service.ProfileService.EntityRepository.FindOneById(user.Profiles["OldHouseUserProfile"]);
                profile.AddPoint(50);
                service.ProfileService.EntityRepository.SaveOne(profile);
            }
        }

        public List<Subscriber> GetHandlerSubscribers()
        {
            var subs = new List<Subscriber>();
            subs.Add(new Subscriber("GrantCheckInEssenceSubscriber", new List<string>() { "GrantCheckInEssenceEvent" }, new List<string>() { "GrantCheckInEssenceHandler" }, null));
            return subs;
        }
    }
}
