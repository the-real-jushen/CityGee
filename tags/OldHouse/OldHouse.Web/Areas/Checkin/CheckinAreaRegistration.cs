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
                "Checkin/{houseId}",
                new { action = "checkin",Controller="checkin"}
            );

            context.MapRoute(
                "NewCheckin",
                "NewCheckin/{houseId}/{dis}",
                new { action = "newCheckin", Controller = "checkin", dis = UrlParameter.Optional }
            );

            context.MapRoute(
                "checkinDetail",
                "Checkin/detail/{id}",
                new { action = "Detail", Controller = "checkin" }
            );

        }
    }
}