using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Jtext103.Auth;
using Jtext103.Identity.Models;
using Jtext103.Identity.Service;
using Jtext103.OldHouse.Business.Models;
using Jtext103.OldHouse.Business.Services;
using OldHouse.Web.App_Start;
using OldHouse.Web.Models;
using Owin;
using Microsoft.Owin;
using OldHouse.Web.Controllers;

namespace OldHouse.Web.Areas.Account.Controllers
{
    public class AccountController : BaseController
    {
       
        // GET: Account/Account
        public ActionResult UserBrief(string id)
        {
            //get the service
            //todo use ioc debendency injection here
            //todo error handing here
            var service = BusinessConfig.MyHouseService;
            var user=service.MyUserManager.FindByIdAsync(Guid.Parse(id)).Result;

            return PartialView("_PartialUserBrief", Mapper.Map<UserDisplayDto>(user));
        }



        #region non-business

        // GET: /Account/
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        // POST: /Account/Login
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //add 7 days to the security stamp
                var loginClaim = await MyService.MyUserManager.Login(model.UserName, model.Password, DateTime.Now.AddDays(7));
                if (loginClaim != null)
                {
                    loginClaim.Remember = model.RememberMe;
                    Jtext103AuthMiddleware<OldHouseUser>.Login(HttpContext.GetOwinContext().Environment, loginClaim);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }


        [HttpPost]
        public ActionResult Logout()
        {
            Jtext103AuthMiddleware<OldHouseUser>.Logout(HttpContext.GetOwinContext().Environment);
            return RedirectToAction("Index", "Home", new { area = "" });
        }



        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            //do not use userManager to create users use the house service, it will create profile for you
            if (ModelState.IsValid)
            {
                var user = Mapper.Map<OldHouseUser>(model);
                var result = await MyService.CreateUserWithProfile(user, new HashSet<string> { OldHouseUserProfile.PROFILENBAME });     
                if (result.IsSuccessful)
                {
                    var loginClaim = await MyService.MyUserManager.Login(model.UserName, model.Password, DateTime.Now.AddDays(7));
                    if (loginClaim != null)
                    {
                        Jtext103AuthMiddleware<OldHouseUser>.Login(HttpContext.GetOwinContext().Environment, loginClaim);
                    }
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    AddErrors(result);
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }




        #region helper
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }



        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        #endregion

        #endregion

    }
}