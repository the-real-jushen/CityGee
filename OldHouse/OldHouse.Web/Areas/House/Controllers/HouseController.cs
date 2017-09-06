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
using OldHouse.Web.Extension;
using Jtext103.Volunteer.VolunteerEvent;
using Jtext103.OldHouse.Business.Models;

namespace OldHouse.Web.Areas.House.Controllers
{
    /// <summary>
    /// house相关功能
    /// </summary>
    [RouteArea("House")]
    public class HouseController : BaseController
    {
        /// <summary>
        /// 获取附近城迹页面
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult Near(int page = 1, int pagesize = 6, string search = "")
        {
            var lastpage = 0;
            Dictionary<string, string> filter = new Dictionary<string, string>();
            //优先获取profile中的city，否则获取cookie中的city
            //string profileCity = (AppUser == null ? null : MyService.GetCurrentCityFormProfile(AppUser.Profiles[OldHouseUserProfile.PROFILENBAME]));
            //string currentCity = (profileCity == null ? CurrentCity : profileCity);
            if (CurrentCity != null)
            {
                filter.Add("city", CurrentCity);
            }
            filter.Add("search", search);
            lastpage = (int)Math.Ceiling(MyService.FilterHouseCount(filter) / (double)pagesize);
            
            ViewBag.PageControl = new PageControl(page, lastpage, pagesize);

            ViewBag.Title = "附近的城迹";
            return View();
        }
        /// <summary>
        /// 获取附近成绩列表
        /// </summary>
        /// <param name="lnt">latitude</param>
        /// <param name="lat">longitude</param>
        /// <param name="page">page number</param>
        /// <param name="pagesize">defualt is 6</param>
        /// <param name="search">搜索条件</param>
        /// <returns></returns>
        public ActionResult RealNear(string lnt, string lat, int page = 1, int pagesize = 6, string search = "")
        {
            Dictionary<string, string> filter = new Dictionary<string, string>();
            //优先获取profile中的city，否则获取cookie中的city
            //string profileCity = (AppUser == null ? null : MyService.GetCurrentCityFormProfile(AppUser.Profiles[OldHouseUserProfile.PROFILENBAME]));
            //string currentCity = (profileCity == null ? CurrentCity : profileCity);
            if (CurrentCity != null)
            {
                filter.Add("city", CurrentCity);
            }
            filter.Add("search", search);
            filter.Add("location", lnt + ";" + lat);
            IEnumerable<Jtext103.OldHouse.Business.Models.House> houses = MyService.FilterHouse(filter, "", true, page, pagesize);
            var dtoHoustlist = new List<HouseBrief>();
            foreach (var house in houses)
            {
                var tHouse = Mapper.Map<HouseBrief>(house);
                var loc1 = HouseService.GetGeoPoint(lnt + ";" + lat);
                var loc2 = HouseService.GetGeoPoint(house.Location.coordinates[0] + ";" + house.Location.coordinates[1]);
                tHouse.DistanceInKm = HouseService.GetDistanceKm(loc1,loc2);
                dtoHoustlist.Add(tHouse);   
            }
            ViewBag.Title = "附近的城迹";
            return PartialView("_PartialHouseList", dtoHoustlist);
        }
        /// <summary>
        /// 获取house详情页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
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
            var house = MyService.FindOneById(houseId);
            if (house == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.Title = house.Name;
                //判断是否显示编辑按钮
                ViewBag.ShowEdit = false;
                if(AppUser!=null && (AppUser.Id.Equals(house.OwnerId) || AppUser.Roles.Contains("Editor")))
                {
                    ViewBag.ShowEdit = true;
                }
                var tHouse = Mapper.Map<HouseDetail>(house);
                tHouse.DistanceInKm = dis;

                //产生ViewAHouse事件
                if (AppUser != null)
                {
                    EventService.Publish("ViewAHouseEvent", house.Id, AppUser.Id);
                }
                else
                {
                    EventService.Publish("ViewAHouseEvent", house.Id, Guid.Empty);
                }
                HouseBrief model = Mapper.Map<HouseBrief>(house);
                return View(tHouse);
            }
        }
        /// <summary>
        /// 获取house简介
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取house列表，按sortbykey排序
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="sortbykey"></param>
        /// <returns></returns>
        public ActionResult HouseList(int page = 1, int pagesize = 6, string search = "", string sortbykey = "CreateTime", bool ascend = false)
        {
            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("search", search);
            IEnumerable<Jtext103.OldHouse.Business.Models.House> houses = MyService.FilterHouse(filter, sortbykey, ascend, page, pagesize);
            var dtoHoustlist = Mapper.Map<IEnumerable<HouseBrief>>(houses);
            return PartialView("_PartialHouseList", dtoHoustlist);
        }

        #region discovery
        /// <summary>
        /// 获取新建/编辑house页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Discover")]
        [Authorize]
        public ActionResult GetEditHousePage(string id = "")
        {
            //return with a House create dto model
            //this view is also used a edit house page
            //it the id is empty then it's create house else is edit
            //todo wan, create the view
            //user this model to handle user discovery house info
            var houseDto = new HouseDiscoverDto();
            if (id != "")
            {
                Jtext103.OldHouse.Business.Models.House house = MyService.FindOneById(new Guid(id));
                houseDto = Mapper.Map<HouseDiscoverDto>(house);
                ViewBag.Title = "编辑" + houseDto.Name;
            }
            else
            {
                houseDto.Id = Guid.NewGuid().ToString();
                houseDto.OwnerId = AppUser.Id.ToString();
                houseDto.IsApproved = "False";
                houseDto.Country = "中国";
                houseDto.Province = "湖北省";
                houseDto.City = "武汉市";
                houseDto.Cover = "/Content/Images/components/noImage.jpg";
                houseDto.Images = new List<string>();
                houseDto.BuiltYear = DateTime.Now.Year;
                ViewBag.Title = "分享你的城迹";
            }
            return View(houseDto);
        }
        /// <summary>
        /// 处理添加/修改house请求
        /// </summary>
        /// <param name="newHouse"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Discover")]
        [Authorize]
        public ActionResult AddOrModifyHouse(HouseDiscoverDto newHouse)
        {
            //for edit a house,we should Authorize
            if (!(new Guid(newHouse.OwnerId).Equals(AppUser.Id) || AppUser.Roles.Contains("Editor")))
            {
                Jtext103.Auth.Jtext103AuthMiddleware<Jtext103.OldHouse.Business.Models.OldHouseUser>.Logout(HttpContext.GetOwinContext().Environment);
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            if (ModelState.IsValid)
            {
                //mapper has bug
                //var house = Mapper.Map<Jtext103.OldHouse.Business.Models.House>(newHouse);
                //house.CodeName = newHouse.Name.GeneratePinYinSlug();  //this is not tested yet! may have bugs

                var house = new Jtext103.OldHouse.Business.Models.House()
                {
                    Name = newHouse.Name,
                    LocationString = newHouse.LocationString,
                    Country = newHouse.Country,
                    Province = newHouse.Province,
                    City = newHouse.City,
                    Abstarct = newHouse.Abstarct,
                    Description = newHouse.Description,
                    Images = newHouse.Images,
                    Cover = newHouse.Cover,
                    Rating = newHouse.Rating,
                    Id = new Guid(newHouse.Id),
                    OwnerId = new Guid(newHouse.OwnerId),
                    IsApproved = Convert.ToBoolean(newHouse.IsApproved),
                    Location = HouseService.GetGeoPoint(newHouse.Lnt + @";" + newHouse.Lat),
                    CodeName = newHouse.Name.GeneratePinYinSlug(),
                    Tags = (newHouse.Tags == null ? new List<string>() : newHouse.Tags.Split(new string[] { "," }, 100, StringSplitOptions.RemoveEmptyEntries).ToList<string>())
                };
                house.ModifyExtraInformation("houseinfo-buildyear", new DateTime(newHouse.BuiltYear, 10, 1));
                //新建house(数据库中找不到)
                if (MyService.FindOneById(new Guid(newHouse.Id)) == null)
                {
                    //产生new house事件
                    EventService.Publish("NewHouseEvent", house.Id, house.OwnerId);
                }
                MyService.SaveOne(house);
                //todo wan, redirect tho the new house details
                return RedirectToAction("HouseDetail",new { id = house.Id});
            }
            if (MyService.FindOneById(new Guid(newHouse.Id)) == null)
            {
                ViewBag.Title = "分享你的城迹";
            }
            else
            {
                ViewBag.Title = "编辑" + newHouse.Name;
            }
            return View(newHouse);
        }
        /// <summary>
        /// 获取用户创建的house列表页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [ActionName("Mine")]
        public ActionResult MyHouses(string id = "", int page = 1, int pagesize = 6, string search = "")
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
            ViewBag.Title = user.NickName + "发现的城迹";
            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("search", search);
            filter.Add("ownerid", id);

            var lastpage = (int)Math.Ceiling(MyService.FilterHouseCount(filter) / (double)pagesize);
            ViewBag.PageControl = new PageControl(page, lastpage, pagesize);

            return View();
        }
        /// <summary>
        /// 获取用户创建的house列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [ActionName("UserDiscoveries")]
        public ActionResult UserDiscoveries(string id, int page = 1, int pagesize = 6, string search = "")
        {
            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("search", search);
            filter.Add("ownerid", id);
            IEnumerable<Jtext103.OldHouse.Business.Models.House> houses = MyService.FilterHouse(filter, "", true, page, pagesize);
            var dtoHoustlist = Mapper.Map<IEnumerable<HouseBrief>>(houses);

            //may not be needed, delete this is you set this in parent controller
            return PartialView("_PartialHouseList", dtoHoustlist);
        }
        #endregion

        public ActionResult ImageThumb(string path)
        {
            return PartialView("_PatialImageThumb", path);
        }


    }
}