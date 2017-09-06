using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jtext103.BlogSystem;
using OldHouse.Web.Models;
using OldHouse.Web.Controllers;
using Jtext103.OldHouse.Business.Models;
using AutoMapper;

namespace OldHouse.Web.Areas.Account.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class UserController : BaseController
    {
        /// <summary>
        /// 用户主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int page = 1, int pagesize = 6, string search = "")
        {
            var lastpage = 0;
            if(search.Equals(""))
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
        /// 获得搜索的用户列表，默认是得分排名列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult RankList(int page = 1, int pagesize = 6, string search = "")
        {
            IEnumerable<OldHouseUser> userList = null;
            if(search.Equals(""))
            {
                userList = MyService.GetAllUsersSortByPoint(page,pagesize);
            }
            else
            {
                userList = MyService.GetUserByNickNameOrUserName(search,page,pagesize);
            }
            var userDtoList = Mapper.Map<IEnumerable<UserInformationDto>>(userList);
            for (int i = 0; i < userDtoList.Count(); i++ )
            {
                userDtoList.ElementAt(i).MyRank = pagesize * (page - 1) + i + 1;
            }
            return PartialView("_PartialUserRank",userDtoList);
        }
    }
}