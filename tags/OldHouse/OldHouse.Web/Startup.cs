using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Jtext103.Auth;
using Jtext103.OldHouse.Business.Models;
using OldHouse.Web.App_Start;
using Owin;

namespace OldHouse.Web
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    public class Startup
    {
        public Startup()
        {


        }

        public void Configuration(IAppBuilder app)
        {
            //this is called after the application_starts
            app.UseJtext103Auth<OldHouseUser>(
                new Jtext103AuthConfig<OldHouseUser>
                {
                    RedirectPth = @"/account/login",
                    UserManager = BusinessConfig.MyHouseService.MyUserManager
                });
        }
    }
}