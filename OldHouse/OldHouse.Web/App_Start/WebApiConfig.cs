using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace OldHouse.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "IsFollowing",
                routeTemplate: "api/account/following/{targetUserId}",
                defaults: new { controller = "Account", action = "following" }
            );

            config.Routes.MapHttpRoute(
                name: "Editor",
                routeTemplate: "api/editor/{action}/{id}",
                defaults: new { controller = "Editor" }
            );

            config.Routes.MapHttpRoute(
                name: "LrfTimes",
                routeTemplate: "api/lrf/times/{type}/{id}",
                defaults: new { controller = "Lrf", action = "Times"}
            );

            config.Routes.MapHttpRoute(
                name: "GetMyRate",
                routeTemplate: "lrf/house/rate/{id}",
                defaults: new { controller = "Lrf", action = "Rate" }
            );

            config.Routes.MapHttpRoute(
                name: "GetUserLrf",
                routeTemplate: "api/lrf/my/{type}/{id}",
                defaults: new { controller = "Lrf", action = "Get" }
            );
           

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApiRest",
                routeTemplate: "api/{controller}"
            );
        }
    }
}
