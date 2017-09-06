using System.Web.Mvc;

namespace OldHouse.Web.Areas.Feed
{
    public class FeedAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "Feed";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Feed_default",
                "Feed/{action}/{id}",
                new { Controller = "Feed", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}