using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Jtext103.Auth;
using OldHouse.Web.Models;

namespace OldHouse.Web.Controllers
{
    public class HomeController : BaseController
    {
        /// <summary>
        /// 对于已登陆用户跳转到新鲜事，未登录用户跳转到登录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (AppUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToAction("Near", "House");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadingStart()
        {
            return PartialView("_PartialLoading");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadingMore()
        {
            return PartialView("_PartialLoadingMore");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult FunGame()
        {
            ViewBag.Title = "Have fun!";
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult WebSiteHistory()
        {
            ViewBag.Title = "网站历史";
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AboutUs()
        {
            ViewBag.Title = "关于我们";
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AppDownload()
        {
            ViewBag.Title = "APP下载";
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CooperativePartner()
        {
            ViewBag.Title = "合作伙伴";
            return View();
        }
    }
}
