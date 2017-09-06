using System.Web.Mvc;

namespace OldHouse.Web.Areas.Feedback
{
    public class FeedbackAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Feedback";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Feedback_default",
                "Feedback/{action}/{id}",
                new { Controller = "Feedback", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}