using System.Web.Mvc;

namespace OldHouse.Web.Areas.Checkin
{
    public class CheckinAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Checkin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //add a checkin to a house or list all its checkins
            context.MapRoute(
                "Checkin",
                "house/Checkin/{houseId}",
                new { action = "checkin",Controller="checkin"}
            );

            context.MapRoute(
                "NewCheckin",
                "house/NewCheckin/{houseId}/{dis}",
                new { action = "newCheckin", Controller = "checkin", dis = UrlParameter.Optional }
            );

            context.MapRoute(
                "checkinDetail",
                "house/Checkin/detail/{id}",
                new { action = "Detail", Controller = "checkin" }
            );

            context.MapRoute(
                "userCheckin",
                "Checkin/user/{id}",
                new { action = "UserCheckins", Controller = "checkin" }
            );
            context.MapRoute(
                "myCheckin",
                "Checkin/Mine/{id}",
                new { action = "Mine", Controller = "checkin", id = UrlParameter.Optional }
            );

        }
    }
}