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
    public class PointIncreaseHandler : IVolunteerEventHandler
    {

        public string Name
        {
            get
            {
                return "PointIncreaseHandler";
            }
        }

        public void HandleEvent(Event oneEvent)
        {
            var service = HouseService.Instence;
            var profile = service.ProfileService.FindOneById((Guid)(oneEvent.Subject));
            var userId = profile.UserId;
            var point = profile.Point;
            string title = "加分啦";
            string myContent = "你现在的分数为" + point.ToString();
            string link = "/Profile/HomePage/" + userId.ToString();
            Message msg = new Message { Title = title, Text = myContent, IsBroadcast = false, MessageFrom = "system", ReceiverId = userId, MessageType = "Feed.PointIncreaseHandler", DestinationLink = link };
        }

        public List<Subscriber> GetHandlerSubscribers()
        {
            var subs = new List<Subscriber>();
            subs.Add(new Subscriber("PointIncreaseSubscriber", new List<string>() { "PointIncreaseEvent" }, new List<string>() { "PointIncreaseHandler" }, null));
            return subs;
        }
    }
}
