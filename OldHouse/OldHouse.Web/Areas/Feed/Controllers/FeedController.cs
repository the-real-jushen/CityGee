using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using OldHouse.Web.Controllers;
using Jtext103.Volunteer.VolunteerMessage;
using OldHouse.Web.Models;

namespace OldHouse.Web.Areas.Feed.Controllers
{
    /// <summary>
    /// Feed功能
    /// </summary>
    public class FeedController : BaseController
    {
        /// <summary>
        /// 返回feed首页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ActionResult Index(int page = 1, int pagesize = 12)
        {
            var lastpage = 0;
            if (AppUser != null)
            {
                ViewBag.Title = AppUser.NickName+"的新鲜事";
                lastpage = (int)Math.Ceiling(MyService.FindBroadcastAndMyFeedCount(AppUser.Id) / (double)pagesize);
            }
            else
            {
                ViewBag.Title = "新鲜事";
                lastpage = (int)Math.Ceiling(MyService.FindBroadcastFeedCount(Guid.Empty) / (double)pagesize);
            }
            ViewBag.PageControl = new PageControl(page, lastpage, pagesize);
            return View();
        }
        /// <summary>
        /// 返回feed列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ActionResult All(int page = 1, int pagesize = 12)
        {
            IEnumerable<Message> feeds = null;
            IEnumerable<FeedDto> feedsDto = null;
            if (AppUser != null)
            {
                feeds = MyService.FindBroadcastAndMyFeeds(AppUser.Id, "Time", false, page, pagesize);
                feedsDto = Mapper.Map<IEnumerable<FeedDto>>(feeds);
            }
            else
            {
                feeds = MyService.FindBroadcastFeeds(Guid.Empty, "Time", false, page, pagesize);
                feedsDto = Mapper.Map<IEnumerable<FeedDto>>(feeds);
            }
            return PartialView("_PartialFeed", feedsDto);
        }
    }
}