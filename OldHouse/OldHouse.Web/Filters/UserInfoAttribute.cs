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
        /// <summary>
        /// fill user log in lifo 
        /// as well as user's client info
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as BaseController;
            if (controller != null)
            {
                controller.SetUpUser();
                controller.GetUserAgent();
                controller.SetCity();
            }
        }
    }
}