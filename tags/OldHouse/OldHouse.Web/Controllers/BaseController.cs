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

namespace OldHouse.Web.Controllers
{
    public class BaseController : Controller
    {
        public HouseService MyService;
        public OldHouseUser AppUser;
        // GET: Base
        public BaseController()
        {
            MyService = BusinessConfig.MyHouseService;         
        }

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


    }
}