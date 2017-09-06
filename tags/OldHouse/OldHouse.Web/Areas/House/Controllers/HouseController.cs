using AutoMapper;
using OldHouse.Web.App_Start;
using OldHouse.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Mvc;
using Jtext103.OldHouse.Business.Services;
using OldHouse.Web.Controllers;

namespace OldHouse.Web.Areas.House.Controllers
{
    [RouteArea("House")]
    public class HouseController : BaseController
    {
        // GET: House/House
        /// <summary>
        /// currently just return all houses
        /// </summary>
        /// <returns></returns>
        //[Route(@"near?page=&pagesize?")]
        public ActionResult Near(int page = 1,int pagesize=6)
        {
            //var houses = BusinessConfig.MyHouseService.Find("", false, 0, 0);
            //var dtoHoustlist = new List<HouseBrief>();
            //foreach (var house in houses)
            //{
            //    dtoHoustlist.Add(Mapper.Map<HouseBrief>(house));
            //}

            //you need to get the last page,then give it to page control
            //paging is done by the parent, since it resive the request thath contains paging info
            var lastpage = (int)Math.Ceiling(MyService.FindAllCount() / (double)pagesize);
            if (lastpage > 1)
            {
                ViewBag.PageControl = new PageControl(page, lastpage, pagesize);
            }

            ViewBag.Title = "附近的老房子";
            return View();
        }

        /// <summary>
        /// get houses near a loction
        /// </summary>
        /// <param name="lnt">latitude</param>
        /// <param name="lat">longitude</param>
        /// <param name="page">page number</param>
        /// <param name="pagesize">defualt is 6</param>
        /// <returns></returns>
        public ActionResult RealNear(string lnt,string lat,int page=1,int pagesize=6)
        {
          
            var houses = BusinessConfig.MyHouseService.FindNear(lnt + ";" + lat, page, pagesize);
            var dtoHoustlist = new List<HouseBrief>();
            foreach (var house in houses)
            {
                var tHouse = Mapper.Map<HouseBrief>(house);
                var loc1 = HouseService.GetGeoPoint(lnt + ";" + lat);
                var loc2 = HouseService.GetGeoPoint(house.Location.coordinates[0] + ";" + house.Location.coordinates[1]);
                tHouse.DistanceInKm = HouseService.GetDistanceKm(loc1,loc2);
                dtoHoustlist.Add(tHouse);   
            }
                    
            ViewBag.Title = "附近的老房子";
            return PartialView("_PartialRealNear",dtoHoustlist);
        }

        //[Route(@"detail/{id}",Name = "houseDetail")]
        public ActionResult HouseDetail(string id, string dis="?")
        {
            Guid houseId;
            try
            {
                houseId = Guid.Parse(id);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;                    
                return Content("bad id");
            }
            var house = BusinessConfig.MyHouseService.FindOneById(houseId);
            if (house == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.Title = house.Name;
                var tHouse = Mapper.Map<HouseDetail>(house);
                tHouse.DistanceInKm = dis;
                return View(tHouse);
            }


        }


        //[Route(@"detail/{id}",Name = "houseDetail")]
        public ActionResult Brief(string id)
        {
            Guid houseId;
            try
            {
                houseId = Guid.Parse(id);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;                    
                return Content("bad id");
            }
            var house = BusinessConfig.MyHouseService.FindOneById(houseId);
            if (house == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.Title = house.Name;
                var tHouse = Mapper.Map<HouseBrief>(house);
                return PartialView("_PartialHouseBrief",tHouse);
            }
        }
    
    }


}