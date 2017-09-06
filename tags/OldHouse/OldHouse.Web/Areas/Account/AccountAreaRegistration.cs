using System.Web.Mvc;

namespace OldHouse.Web.Areas.Account
{
    public class AccountAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Account";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Account_default",
            //    "Account/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
                "Account",
                "Account/{action}",
                new { action = "Index",Controller="Account"}
            );
        }
    }
}