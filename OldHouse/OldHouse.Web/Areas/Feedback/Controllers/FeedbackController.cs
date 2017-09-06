using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using OldHouse.Web.Controllers;
using OldHouse.Web.App_Start;
using Jtext103.OldHouse.Business.Models;
using OldHouse.Web.Models;

namespace OldHouse.Web.Areas.Feedback.Controllers
{
    /// <summary>
    /// 用户反馈功能
    /// </summary>
    public class FeedbackController : BaseController
    {
        /// <summary>
        /// 用户反馈列表页面
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取用户反馈列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult All(int page=1,int pagesize=6)
        {
            if (!AppUser.Roles.Contains("Developer"))
            {
                return View("Error");
            }
            //sorting
            var feedbacks = BusinessConfig.MyFeedbackService.FindAllFeedBackFor("CreatedTime", false, page, pagesize).Cast<FeedBackEntity>();
            var feedbacksDto = Mapper.Map<IEnumerable<FeedBackDto>>(feedbacks);

            //paging
            var lastpage = (int)Math.Ceiling(BusinessConfig.MyFeedbackService.FindAllFeedBackCountFor() / (double)pagesize);
            
            var pc = new PageControl(page, lastpage, pagesize);
            //this is a partial view so pass in the route info
            pc.RouteName = "Feedback_default";
            pc.RouteValue = new Dictionary<string, object> { { "action", "All" } };
            ViewBag.PageControl = pc;

            return PartialView("_PartialFeedback", feedbacksDto);
        }
    }
}