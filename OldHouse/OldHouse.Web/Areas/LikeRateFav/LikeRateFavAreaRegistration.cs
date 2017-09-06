using System.Web.Mvc;

namespace OldHouse.Web.Areas.LikeRateFav
{
    public class LikeRateFavAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "LikeRateFav";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MyLiked",
                "LikeRateFav/{action}/{type}/{id}",
                new { action = "MyLiked", Controller = "LikeRateFav", type="houses" }
            );
            context.MapRoute(
                "MyLikedHouses",
                "LikeRateFav/{action}/{id}",
                new { action = "UserLikedHouses", Controller = "LikeRateFav" }
            );
            context.MapRoute(
                "MyLikedCheckins",
                "LikeRateFav/{action}/{id}",
                new { action = "UserLikedCheckins", Controller = "LikeRateFav" }
            );
        }
    }
}