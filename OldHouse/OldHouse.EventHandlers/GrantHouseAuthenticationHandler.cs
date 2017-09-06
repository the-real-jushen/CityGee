using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.OldHouse.Business.Models;
using Jtext103.OldHouse.Business.Services;
using Jtext103.Volunteer.VolunteerEvent;
using Jtext103.Volunteer.VolunteerMessage;


namespace OldHouse.EventHandlers
{
    /// <summary>
    /// 官方认证house
    /// </summary>
    public class GrantHouseAuthenticationHandler : IVolunteerEventHandler
    {
        public string Name
        {
            get
            {
                return "GrantHouseAuthenticationHandler";
            }
        }

        public void HandleEvent(Event oneEvent)
        {
            var service = HouseService.Instence;
            var house = service.FindOneById((Guid)oneEvent.Subject);
            Guid userId = house.OwnerId;
            string title = "官方认证";
            string myContent = "您发现的老房子【" + house.Name + "】刚刚被官方认证了！";
            string link = "/House/detail/" + house.Id.ToString();
            List<string> picture = new List<string>();
            if (house.Cover != null)
            {
                picture.Add(house.Cover);
            }
            Message msg = new Message { Title = title, Text = myContent, IsBroadcast = false, MessageFrom = "system", ReceiverId = userId, MessageType = "Feed.GrantHouseAuthenticatio", DestinationLink = link, Pictures = picture };
            service.FeedService.SendMessage(msg);

            //为该用户加100分
            var user = service.MyUserManager.FindByIdAsync(userId).Result;
            if (user != null)
            {
                var profile = service.ProfileService.EntityRepository.FindOneById(user.Profiles["OldHouseUserProfile"]);
                profile.AddPoint(100);
                service.ProfileService.EntityRepository.SaveOne(profile);
            }
        }

        public List<Subscriber> GetHandlerSubscribers()
        {
            var subs = new List<Subscriber>();
            subs.Add(new Subscriber("GrantHouseAuthenticationSubscriber", new List<string>() { "GrantHouseAuthenticationEvent" }, new List<string>() { "GrantHouseAuthenticationHandler" }, null));
            return subs;
        }
    }
}
