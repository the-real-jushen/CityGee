using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OldHouse.Web.Areas.Account.Controllers;
using OldHouse.Web.Controllers;

namespace OldHouse.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var ns = new String[] { typeof(AccountController).Namespace, typeof(HomeController).Namespace };
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces:ns
            );
        }
    }
}
