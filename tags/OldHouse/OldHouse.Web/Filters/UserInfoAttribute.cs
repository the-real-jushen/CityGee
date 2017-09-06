using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OldHouse.Web.Controllers;

namespace OldHouse.Web.Filters
{
    public class UserInfoAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as BaseController;
            if (controller != null)
            {
                controller.SetUpUser();
            }

        }
    }
}