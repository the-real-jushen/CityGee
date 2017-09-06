using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Jtext103.Auth;
using Jtext103.OldHouse.Business.Models;
using Jtext103.OldHouse.Business.Services;
using OldHouse.Web.App_Start;
using OldHouse.Web.Models;
using Jtext103.BlogSystem;

namespace OldHouse.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseController : Controller
    {
        public HouseService MyService;
        public OldHouseUser AppUser;
        public string CurrentCity;
        public string CurrentProvince;
        /// <summary>
        /// 
        /// </summary>
        public BaseController()
        {
            MyService = BusinessConfig.MyHouseService;
        }
        /// <summary>
        /// 
        /// </summary>
        public void SetUpUser()
        {
            try
            {
                AppUser = MyService.MyUserManager.FindByIdAsync(Guid.Parse(User.Identity.GetUserId())).Result;
                ViewBag.User = Mapper.Map<UserDisplayDto>(AppUser);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// check if user is using a android client or something
        /// </summary>
        public void GetUserAgent()
        {
            try
            {
                if (Request.UserAgent.Contains("CityGee WebApp"))
                {
                    ViewBag.UserClient = "CityGee WebApp";
                }
                else
                {
                    ViewBag.UserClient = "Browser";
                }
            }
            catch 
            {
                    
                ViewBag.UserClient = "Unknown";
            }

        }
        /// <summary>
        /// 
        /// </summary>
        public void SetCity()
        {
            if(AppUser != null)
            {
                var profileCity = MyService.GetCurrentCityFormProfile(AppUser.Profiles[OldHouseUserProfile.PROFILENBAME]);
                var profileProvince = MyService.GetCurrentProvinceFormProfile(AppUser.Profiles[OldHouseUserProfile.PROFILENBAME]);
                CurrentCity = profileCity;
                CurrentProvince = profileProvince;
            }
            else
            {
                if (Request.Cookies["citygee-currentcity"] != null)
                {
                    CurrentCity = HttpUtility.UrlDecode(Request.Cookies["citygee-currentcity"].Value);
                    CurrentProvince = HttpUtility.UrlDecode(Request.Cookies["citygee-currentprovince"].Value);
                }
                else
                {
                    CurrentCity = "武汉市";
                    CurrentProvince = "湖北省";
                }
            }
            ViewBag.CurrentCity = CurrentCity;
            ViewBag.CurrentProvince = CurrentProvince;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected LrfDto GetLrfDto(Guid id)
        {
            var lrfDto = new LrfDto {Id = id};
            lrfDto.LrfCount = MyService.LrfService.GetLRFCount(id, LRFType.Like);
            if (AppUser != null)
            {
                lrfDto.DidILrf = MyService.LrfService.DoILikeRateFav(AppUser.Id, id, LRFType.Like);
            }
            return lrfDto;
        }
    }
}