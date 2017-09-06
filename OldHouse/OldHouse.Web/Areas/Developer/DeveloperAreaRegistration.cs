using System.Web.Mvc;

namespace OldHouse.Web.Areas.Developer
{
    public class DeveloperAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Developer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Developer_default",
                "Developer/{action}/{id}",
                new { controller = "Developer", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}