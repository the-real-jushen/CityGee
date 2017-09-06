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
        public ActionResult Index()
        {
            //ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Loading()
        {
            return PartialView("_PartialLoading");
        }
    }
}
