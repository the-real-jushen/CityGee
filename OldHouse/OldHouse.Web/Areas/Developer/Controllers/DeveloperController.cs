using AutoMapper;
using Jtext103.OldHouse.Business.Models;
using Jtext103.Volunteer.VolunteerMessage;
using OldHouse.Web.App_Start;
using OldHouse.Web.Controllers;
using OldHouse.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OldHouse.Web.Areas.Developer.Controllers
{
    /// <summary>
    /// 程序猿工作站
    /// </summary>
    public class DeveloperController : BaseController
    {
        /// <summary>
        /// 获取程序猿操作首页
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            if (!AppUser.Roles.Contains("Developer"))
            {
                Jtext103.Auth.Jtext103AuthMiddleware<Jtext103.OldHouse.Business.Models.OldHouseUser>.Logout(HttpContext.GetOwinContext().Environment);
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            return View();
        }
    }
}