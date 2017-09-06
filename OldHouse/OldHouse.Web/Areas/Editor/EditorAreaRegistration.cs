using System.Web.Mvc;

namespace OldHouse.Web.Areas.Editor
{
    public class EditorAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Editor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Editor_default",
                "Editor/{action}/{id}",
                new { controller = "Editor", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}