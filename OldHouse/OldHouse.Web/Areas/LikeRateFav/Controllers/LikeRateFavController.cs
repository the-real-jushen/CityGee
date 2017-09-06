using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jtext103.BlogSystem;
using Jtext103.OldHouse.Business.Models;
using OldHouse.Web.Models;
using OldHouse.Web.Controllers;
using AutoMapper;

namespace OldHouse.Web.Areas.LikeRateFav.Controllers
{
    /// <summary>
    /// Favorite功能
    /// </summary>
    public class LikeRateFavController : BaseController
    {
        /// <summary>
        /// 获取点赞按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("LikeMe")]
        public ActionResult LikeMe(Guid id)
        {
            var lrfDto = GetLrfDto(id);
            return PartialView("_PartialLike",lrfDto);
        }
        /// <summary>
        /// 获取用户赞过的house或checkin列表页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ActionName("MyLiked")]
        public ActionResult MyLiked(string id = "", string type = "houses", int page = 1, int pagesize = 6)
        {
            if (id.Equals(""))
            {
                id = AppUser.Id.ToString();
            }
            var user = MyService.MyUserManager.FindByIdAsync(new Guid(id)).Result;
            UserInformationDto model = Mapper.Map<UserInformationDto>(user);
            if (AppUser != null && model.Id.Equals(AppUser.Id))
            {
                model.Who = "我";
            }
            ViewBag.Visitor = model;
            ViewBag.LikedHouseCount = MyService.FindLikedHouseCountByUser(user.Id);
            ViewBag.LikedCheckinCount = MyService.CheckInService.FindLikedBlogPostCountByUser(user.Id);
            ViewBag.Title = user.NickName + "的点赞";
            int lastpage = 0;
            switch(type)
            {
                case "houses":
                    lastpage = (int)Math.Ceiling(ViewBag.LikedHouseCount / (double)pagesize);
                    break;
                case "checkins":
                    lastpage = (int)Math.Ceiling(ViewBag.LikedCheckinCount / (double)pagesize);
                    break;
                default:
                    break;
            }

            ViewBag.PageControl = new PageControl(page, lastpage, pagesize);

            return View("Like");
        }
        /// <summary>
        /// 获取用户赞过的house列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ActionName("UserLikedHouses")]
        public ActionResult UserLikedHouses(string id = "", int page = 1, int pagesize = 6)
        {
            if (id.Equals(""))
            {
                id = AppUser.Id.ToString();
            }

            var houses = MyService.FindLikedHouseByUser(new Guid(id), page, pagesize);

            var dtoHoustlist = Mapper.Map<IEnumerable<HouseBrief>>(houses);

            return PartialView("_PartialHouseList", dtoHoustlist);
        }
        /// <summary>
        /// 获取用户赞过的checkin列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ActionName("UserLikedCheckins")]
        public ActionResult UserLikedCheckins(string id = "", int page = 1, int pagesize = 6)
        {
            if (id.Equals(""))
            {
                id = AppUser.Id.ToString();
            }

            var checkins = MyService.CheckInService.FindLikedBlogPostByUser(new Guid(id), page, pagesize);
            var checkinsDto = Mapper.Map<IEnumerable<CheckInDto>>(checkins);

            return PartialView("_PartialCheckInHouseList", checkinsDto);
        }
        /// <summary>
        /// 获取用户被赞过的house或checkin列表页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ActionName("BeLiked")]
        public ActionResult BeLiked(string id = "", string type = "houses", int page = 1, int pagesize = 6)
        {
            if (id.Equals(""))
            {
                id = AppUser.Id.ToString();
            }
            var user = MyService.MyUserManager.FindByIdAsync(new Guid(id)).Result;
            ViewBag.UserId = user.Id;
            ViewBag.LikedHouseCount = MyService.FindLikedHouseCountByUser(user.Id);
            ViewBag.LikedCheckinCount = MyService.CheckInService.FindLikedBlogPostCountByUser(user.Id);
            ViewBag.Title = user.NickName + "收获的点赞";
            int lastpage = 0;
            switch (type)
            {
                case "houses":
                    //lastpage = (int)Math.Ceiling(ViewBag.LikedHouseCount / (double)pagesize);
                    break;
                case "checkins":
                    //lastpage = (int)Math.Ceiling(ViewBag.LikedCheckinCount / (double)pagesize);
                    break;
                default:
                    break;
            }
            
            ViewBag.PageControl = new PageControl(page, lastpage, pagesize);

            return View("Like");
        }
        /// <summary>
        /// 获取用户被赞过的house列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ActionName("BeLikedHouses")]
        public ActionResult BeLikedHouses(string id = "", int page = 1, int pagesize = 6)
        {
            if (id.Equals(""))
            {
                id = AppUser.Id.ToString();
            }
            throw new NotImplementedException();
            var houses = MyService.FindLikedHouseByUser(new Guid(id), page, pagesize);

            var dtoHoustlist = Mapper.Map<IEnumerable<HouseBrief>>(houses);

            return PartialView("_PartialHouseList", dtoHoustlist);
        }
        /// <summary>
        /// 获取用户被赞过的checkin列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ActionName("BeLikedCheckins")]
        public ActionResult BeLikedCheckins(string id = "", int page = 1, int pagesize = 6)
        {
            if (id.Equals(""))
            {
                id = AppUser.Id.ToString();
            }
            throw new NotImplementedException();
            var checkins = MyService.CheckInService.FindLikedBlogPostByUser(new Guid(id), page, pagesize);
            var checkinsDto = Mapper.Map<IEnumerable<CheckInDto>>(checkins);

            return PartialView("_PartialCheckInList", checkinsDto);
        }
    }
}