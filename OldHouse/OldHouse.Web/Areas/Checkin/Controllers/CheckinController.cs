using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Jtext103.OldHouse.Business.Models;
using Jtext103.OldHouse.Business.Services;
using OldHouse.Web.App_Start;
using OldHouse.Web.Controllers;
using OldHouse.Web.Models;

namespace OldHouse.Web.Areas.Checkin.Controllers
{
    /// <summary>
    /// 签到功能
    /// </summary>
    public class CheckinController : BaseController
    {
        /// <summary>
        /// 处理添加签到请求
        /// </summary>
        /// <param name="checkInDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult CheckIn(CheckInDto checkInDto)
        {
            if (ModelState.IsValid)
            {
                //get the service
                //todo use ioc debendency injection here
                var service = MyService;
                //todo replace this with real authentication and users
                checkInDto.UserId = AppUser.Id;
                //get the real check in object, and populate the useful field
                //var chekcin = Mapper.Map<CheckIn>(checkInDto);
                var asset = Mapper.Map<List<Jtext103.BlogSystem.Asset>>(checkInDto.Images);
                var chekcin = new Jtext103.OldHouse.Business.Models.CheckIn(new Jtext103.BlogSystem.BasicUser { Id = checkInDto.UserId }, checkInDto.TargetId, checkInDto.Titile, checkInDto.Content, asset, HouseService.GetGeoPoint(checkInDto.Lnt + @";" + checkInDto.Lat));
                //the house id is redeundent
                service.CheckInHouse(chekcin.TargetId, chekcin);
                return RedirectToRoute("HouseDetail", new {id = checkInDto.TargetId, dis = checkInDto.Distance});
            }
            else
            {
                ModelState.AddModelError("", "请一定说点什么吧。");
                return View("NewCheckIn", checkInDto);
            }
        }
        /// <summary>
        /// 获取新建签到页面
        /// </summary>
        /// <param name="houseId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult NewCheckIn(string houseId,string dis)
        {
            //todo error handling
            var targetId = Guid.Parse(houseId);
            var house = MyService.FindOneById(targetId);
            var model = Mapper.Map<HouseBrief>(house);
            return View("NewCheckIn", new CheckInDto { Titile = "MyCheckIn", TargetId = targetId, Distance = dis, HouseName = house.Name});
        }
        /// <summary>
        /// 获取某house下的签到列表
        /// </summary>
        /// <param name="houseId"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("checkin")]
        public ActionResult ListCheckInFor(string houseId,int page=1,int pagesize=3)
        {
            //sorting
            var checkins=MyService.ListCheckInsFor(new Guid(houseId),page,pagesize).Cast<CheckIn>();
            var checkinsDto = Mapper.Map<IEnumerable<CheckInDto>>(checkins);

            //paging
            var lastpage = (int)Math.Ceiling(MyService.GetCheckInCountFor(new Guid(houseId)) / (double)pagesize);

            var pc=new PageControl(page, lastpage, pagesize);
            //this is a partial view so pass in the route info
            pc.RouteName = "Checkin";
            pc.RouteValue=new Dictionary<string, object>{{"houseId",houseId}};
            ViewBag.PageControl = pc;

            return PartialView("_PartialCheckInList",checkinsDto);
        }
        /// <summary>
        /// 获取签到详情页面
        /// </summary>
        /// <param name="id">the checkin id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail(string id)
        {
            var service = BusinessConfig.MyHouseService;
            //sorting
            var checkin = service.CheckInService.FindOneById(Guid.Parse(id));
            var checkinDto = Mapper.Map<CheckInDto>(checkin);
            return View("checkinDetail",checkinDto);
        }
        /// <summary>
        /// 获取我的签到列表页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ActionName("Mine")]
        public ActionResult MyCheckins(string id = "", int page = 1, int pagesize = 6)
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
            ViewBag.Title = user.NickName + "的签到";
            var lastpage = (int)Math.Ceiling(MyService.FindChenkInCountByUser(user.Id) / (double)pagesize);

            ViewBag.PageControl = new PageControl(page, lastpage, pagesize);

            return View();
        }
        /// <summary>
        /// 获取我的签到列表
        /// checkin/user/{id}
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        [ActionName("UserCheckins")]
        public ActionResult UserCheckins(string id = "", int page = 1, int pagesize = 6)
        {
            if (id.Equals(""))
            {
                id = AppUser.Id.ToString();
            }
            var checkins = MyService.FindChenkInByUser(new Guid(id), page, pagesize);

            var checkinsDto = Mapper.Map<IEnumerable<CheckInDto>>(checkins);

            //paging
            var lastpage = (int)Math.Ceiling(MyService.FindChenkInCountByUser(new Guid(id)) / (double)pagesize);

            var pc = new PageControl(page, lastpage, pagesize);
            //this is a partial view so pass in the route info
            pc.RouteName = "userCheckin";
            pc.RouteValue = new Dictionary<string, object> { { "id", id } };
            ViewBag.PageControl = pc;

            return PartialView("_PartialCheckInHouseList", checkinsDto);
        }

    }
}