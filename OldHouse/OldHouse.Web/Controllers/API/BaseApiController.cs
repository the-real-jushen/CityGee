using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Jtext103.Auth;
using Jtext103.OldHouse.Business.Models;
using Jtext103.OldHouse.Business.Services;
using OldHouse.Web.App_Start;
using OldHouse.Web.Models;

namespace OldHouse.Web.Controllers.API
{
    public class BaseApiController : ApiController
    {
        public HouseService MyService;
        public OldHouseUser AppUser;
        // GET: Base
        public BaseApiController()
        {
            MyService = BusinessConfig.MyHouseService;         
        }

        public void SetUpUser()
        {
            try
            {
                AppUser = MyService.MyUserManager.FindByIdAsync(Guid.Parse(User.Identity.GetUserId())).Result;
            }
            catch (Exception)
            {

            }
        }
    }
}
