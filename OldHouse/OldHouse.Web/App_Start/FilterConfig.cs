using System.Web;
using System.Web.Mvc;
using OldHouse.Web.Filters;

namespace OldHouse.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new UserInfoAttribute());
        }

        public static void RegisterApiGlobalFilters(System.Web.Http.Filters.HttpFilterCollection filters)
        {
            filters.Add(new UserInforApiAttribute());
        }
    }
}
