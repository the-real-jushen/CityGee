using AutoMapper;
using Jtext103.OldHouse.Business.Models;
using OldHouse.Web.Controllers;
using OldHouse.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OldHouse.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 管理员工作站
    /// </summary>
    public class AdminController : BaseController
    {
        /// <summary>
        /// 获取管理员首页
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            if (!AppUser.Roles.Contains("Admin"))
            {
                Jtext103.Auth.Jtext103AuthMiddleware<Jtext103.OldHouse.Business.Models.OldHouseUser>.Logout(HttpContext.GetOwinContext().Environment);
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            return View();
        }
        /// <summary>
        /// 获取用户管理页面
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult UserManagement(int page = 1, int pagesize = 6, string search = "")
        {
            var lastpage = 0;
            if (search.Equals(""))
            {
                lastpage = (int)Math.Ceiling(MyService.GetAllUserCount() / (double)pagesize);
            }
            else
            {
                lastpage = (int)Math.Ceiling(MyService.GetUserCountByNickNameOrUserName(search) / (double)pagesize);
            }
            ViewBag.PageControl = new PageControl(page, lastpage, pagesize);
            return View();
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult AllUser(int page = 1, int pagesize = 6, string search = "")
        {
            IEnumerable<OldHouseUser> userList = null;
            if (search.Equals(""))
            {
                userList = MyService.MyUserManager.UserRepository.FindAll("NickName", true, page, pagesize);
            }
            else
            {
                userList = MyService.GetUserByNickNameOrUserName(search, page, pagesize);
            }
            IEnumerable<UserDisplayDto> users = Mapper.Map<IEnumerable<UserDisplayDto>>(userList);
            return PartialView("_PartialFollowList", users);
        }
    }
}