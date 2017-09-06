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
    /// 获取（被）关注用户
    /// </summary>
    public class FollowController : BaseController
    {
        /// <summary>
        /// 获取关注按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("FollowMe")]
        public ActionResult GetView(Guid id)
        {
            var lrfDto = new LrfDto { Id = id };
            if (AppUser != null)
            {
                lrfDto.DidILrf = MyService.AmIFollowing(id, AppUser.Id);
            }
            return PartialView("_PartialFollow", lrfDto);
        }
        /// <summary>
        /// 我关注的用户列表页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("MyFollow")]
        public ActionResult GetMyFollow(string id = "")
        {
            OldHouseUser user = null;
            if (id.Equals(""))
            {
                user = AppUser;
            }
            else
            {
                user = MyService.MyUserManager.FindByIdAsync(new Guid(id)).Result;
            }
            //foreach (var followId in MyService.GetAllFollowingIds(user.Id))
            //{
            //    OldHouseUser followUser = MyService.MyUserManager.FindByIdAsync(MyService.ProfileService.FindOneById(followId).UserId).Result;
            //    follows.Add(followUser);
            //}
            UserInformationDto model = Mapper.Map<UserInformationDto>(user);
            if (AppUser != null && model.Id.Equals(AppUser.Id))
            {
                model.Who = "我";
            }
            ViewBag.Visitor = model;
            IEnumerable<UserDisplayDto> users = Mapper.Map<IEnumerable<UserDisplayDto>>(MyService.GetAllFollowingUser(user.Id));
            ViewBag.Title = user.NickName + "的关注";
            ViewBag.Type = "关注";
            return View("Follow", users);
        }
        /// <summary>
        /// 我的粉丝列表页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("ByFollow")]
        public ActionResult ByFollow(string id = "")
        {
            OldHouseUser user = null;
            if (id.Equals(""))
            {
                user = AppUser;
            }
            else
            {
                user = MyService.MyUserManager.FindByIdAsync(new Guid(id)).Result;
            }
            List<OldHouseUser> follows = new List<OldHouseUser>();
            foreach (var one in MyService.MyUserManager.UserRepository.FindAll())
            {
                if (MyService.AmIFollowing(user.Id, one.Id))
                {
                    follows.Add(one);
                }
            }
            IEnumerable<UserInformationDto> users = Mapper.Map<IEnumerable<UserInformationDto>>(follows);
            ViewBag.Title = user.NickName + "的粉丝";
            ViewBag.Type = "粉丝";
            return View("Follow", users);
        }
    }
}