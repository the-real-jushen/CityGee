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
    /// 添加了新house
    /// </summary>
    public class NewHouseHandler : IVolunteerEventHandler
    {
        public List<Subscriber> GetHandlerSubscribers()
        {
            var subs = new List<Subscriber>();
            subs.Add(new Subscriber("NewHouseSubscriber", new List<string>() { "NewHouseEvent" }, new List<string>() { "NewHouseHandler" }, null));
            return subs;
        }

        public void HandleEvent(Event oneEvent)
        {
            var service = HouseService.Instence;
            House house = service.FindOneById((Guid)oneEvent.Subject);
            OldHouseUser user = service.MyUserManager.FindByIdAsync((Guid)oneEvent.Sender).Result;
            //string myContent = user.NickName + "(" + user.UserName + ")刚刚发现了新的老房子！";
            string myContent = "【" + user.NickName + "(" + user.UserName + ")】有了新发现：【" + house.Name + "】";
            string title = "新的老房子";
            string link = "/House/detail/" + house.Id.ToString();
            List<string> picture = new List<string>();
            if (house.Cover != null)
            {
                picture.Add(house.Cover);
            }
            var msg = new Message { Title = title, Text = myContent, IsBroadcast = true, MessageFrom = user.Id.ToString(), ReceiverId = Guid.Empty, MessageType = "Feed.NewHouse", DestinationLink = link, Pictures = picture };
            service.FeedService.SendMessage(msg);
            //为新old house创建者加20分
            var profile = service.ProfileService.EntityRepository.FindOneById(user.Profiles["OldHouseUserProfile"]);
            profile.AddPoint(20);
            service.ProfileService.EntityRepository.SaveOne(profile);
        }

        public string Name
        {
            get
            {
                return "NewHouseHandler";
            }
        }
    }
}
