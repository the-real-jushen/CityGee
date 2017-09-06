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
    public class CheckinController : BaseController
    {
        
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
                var user = AppUser;
                checkInDto.UserId = user.Id;
                //get the real check in object, and populate the useful field
                var chekcin = Mapper.Map<CheckIn>(checkInDto);
                chekcin.CreateTime = DateTime.Now;
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
        /// get a new check in view for user
        /// /newcheckin/{houseId}
        /// </summary>
        /// <param name="houseId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult NewCheckIn(string houseId,string dis)
        {
            //todo error handling
            var targetId = Guid.Parse(houseId);
            var houseName = MyService.FindOneById(targetId).Name;
            return View("NewCheckIn", new CheckInDto { Titile = "MyCheckIn", TargetId = targetId, Distance=dis,HouseName = houseName});
        }

        /// <summary>
        /// list all checkins for a house id
        /// /checkin/{houseId}
        /// </summary>
        /// <param name="houseId"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("checkin")]
        public ActionResult ListCheckInFor(string houseId,int page=1,int pagesize=2)
        {
            var service = BusinessConfig.MyHouseService;
            //sorting
            var checkins=service.ListCheckInsFor(new Guid(houseId),page,pagesize).Cast<CheckIn>();
            var checkinsDto = Mapper.Map<IEnumerable<CheckInDto>>(checkins);

            //paging
            var lastpage = (int)Math.Ceiling(service.GetCheckInCountFor(new Guid(houseId)) / (double)pagesize);
            if (lastpage > 1)
            {
                var pc=new PageControl(page, lastpage, pagesize);
                //this is a partial view so pass in the route info
                pc.RouteName = "Checkin";
                pc.RouteValue=new Dictionary<string, object>{{"houseId",houseId}};
                ViewBag.PageControl = pc;
            }


            return PartialView("_PartialCheckInList",checkinsDto);
        }


        /// <summary>
        /// get a detailed checkin 
        /// /checkin/Detail/{Id}
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


    }
}