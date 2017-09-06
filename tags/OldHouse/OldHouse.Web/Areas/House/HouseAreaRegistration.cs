using System.Web.Mvc;

namespace OldHouse.Web.Areas.House
{
    public class HouseAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "House";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "House_default",
            //    "House/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
                "houseDetail",
                "House/detail/{id}/{dis}",
                new { action = "HouseDetail", Controller = "House" ,dis=UrlParameter.Optional}
            );

            context.MapRoute(
                "houseNear",
                "House/near",
                new { action = "Near", Controller = "House" }
            );

            context.MapRoute(
                "houseRealNear",
                "House/realnear/{lnt}/{lat}",
                new { action = "RealNear", Controller = "House" }
            );

            context.MapRoute(
                "houseBrief",
                "House/Brief/{id}",
                new { action = "Brief", Controller = "House" }
            );

        }
    }
}