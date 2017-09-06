using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using OldHouse.Web.Controllers.API;

namespace OldHouse.Web.Filters
{
    public class UserInforApiAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            var controller= actionContext.ControllerContext.Controller as BaseApiController;
            if (controller != null)
            {
                controller.SetUpUser();
            }
        }

        
    }
}