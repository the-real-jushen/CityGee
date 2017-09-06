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
    /// 取消关注
    /// </summary>
    public class StopFollowHandler : IVolunteerEventHandler
    {
        public string Name
        {
            get
            {
                return "StopFollowHandler";
            }
        }

        public void HandleEvent(Event oneEvent)
        {
            var service = HouseService.Instence;
            var followeeUid = service.FollowService.FindOneById((Guid)(oneEvent.Subject)).UserId;
            var followeeUser = service.MyUserManager.FindByIdAsync(followeeUid).Result;
            //followee扣5分
            var profile = service.ProfileService.EntityRepository.FindOneById(followeeUser.Profiles["OldHouseUserProfile"]);
            profile.MinusPoint(5);
            service.ProfileService.EntityRepository.SaveOne(profile);
        }

        public List<Subscriber> GetHandlerSubscribers()
        {
            var subs = new List<Subscriber>();
            subs.Add(new Subscriber("StopFollowSubscriber", new List<string>() { "StopFollowEvent" }, new List<string>() { "StopFollowHandler" }, null));
            return subs;
        }
    }
}
