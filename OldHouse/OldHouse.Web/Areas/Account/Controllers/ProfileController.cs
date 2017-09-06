using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OldHouse.Web.Controllers;
using Jtext103.OldHouse.Business.Models;
using OldHouse.Web.Models;
using AutoMapper;

namespace OldHouse.Web.Areas.Account.Controllers
{
    /// <summary>
    /// whe user click on his avatar it will bring the user to one of these page
    /// this shows many info, with tabs, tab is not real tab but different pages
    /// </summary>
    public class ProfileController : BaseController
    {
        /// <summary>
        /// 获取用户主页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("HomePage")]
        public ActionResult GetHomePage(string id = "")
        {
            OldHouseUser user = null;
            if(id.Equals(""))
            {
                user = AppUser;
            }
            else
            {
                user = MyService.MyUserManager.FindByIdAsync(new Guid(id)).Result;
            }
            UserInformationDto model = Mapper.Map<UserInformationDto>(user);
            int count = MyService.GetProfile(user.Profiles[OldHouseUserProfile.PROFILENBAME]).FollowerCount;
            if (AppUser != null && model.Id.Equals(AppUser.Id))
            {
                model.Who = "我";
            }
            ViewBag.Title = model.NickName + "的主页";
            ViewBag.Visitor = model;
            return View(model);
        }
        /// <summary>
        /// 登录用户获取设置页面
        /// </summary>
        /// <returns></returns>
        [ActionName("Setting")]
        [Authorize]
        public ActionResult GetSetting()
        {
            UserInformationDto model = Mapper.Map<UserInformationDto>(AppUser);
            model.Who = "我";
            ViewBag.Title = "个人设置";
            ViewBag.Visitor = model;
            ViewBag.UserProfile = new UserProfileDto
            {
                UserName = AppUser.UserName,
                NickName = AppUser.NickName,
                Sex = AppUser.sex,
                ProfileProvince = MyService.GetCurrentProvinceFormProfile(AppUser.Profiles[OldHouseUserProfile.PROFILENBAME]),
                ProfileCity = MyService.GetCurrentCityFormProfile(AppUser.Profiles[OldHouseUserProfile.PROFILENBAME])
            };
            return View();
        }
        /// <summary>
        /// 处理用户修改资料请求
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult ModifyProfile(UserProfileDto model)
        {
            if (ModelState.IsValid)
            {
                AppUser.UserName = model.UserName;
                AppUser.NickName = model.NickName;
                AppUser.sex = model.Sex;
                MyService.MyUserManager.UserRepository.SaveOne(AppUser);
                MyService.AddOrModifyCurrentProvinceForProfile(AppUser.Profiles[OldHouseUserProfile.PROFILENBAME], model.ProfileProvince);
                MyService.AddOrModifyCurrentCityForProfile(AppUser.Profiles[OldHouseUserProfile.PROFILENBAME], model.ProfileCity);
                ViewBag.Information = "资料修改成功！";
            }
            UserInformationDto user = Mapper.Map<UserInformationDto>(AppUser);
            user.Who = "我";
            ViewBag.Title = "个人设置";
            ViewBag.Visitor = user;
            ViewBag.UserProfile = model;
            return View("Setting");
        }
    }
}