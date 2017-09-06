using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Jtext103.ImageHandler;
using OldHouse.Web.Models;
using Jtext103.Identity.Models;
using OldHouse.Web.App_Start;
using AutoMapper;
using System.Threading.Tasks;

namespace OldHouse.Web.Controllers.API
{
    [Authorize]
    public class AccountController : BaseApiController
    {
        //todo login and register maybe for apps
        [HttpGet]
        [ActionName("AppUser")]
        public HttpResponseMessage GetAppUser()
        {
            if(AppUser != null)
            {
                StringWriter tw = new StringWriter();
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Serialize(tw, AppUser, AppUser.GetType());
                return new HttpResponseMessage { Content = new StringContent(tw.ToString(), System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized, Content = new StringContent("请登录", System.Text.Encoding.GetEncoding("UTF-8"), "application/text") };
        }

        #region follow
        /// <summary>
        /// Am i following this user
        /// </summary>
        /// <param name="targetUserId">the followee user id</param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("following")]
        public bool AmIFollowing(Guid targetUserId)
        {
            if(AppUser != null)
            {
                var followerUserId = AppUser.Id;
                return MyService.AmIFollowing(targetUserId, followerUserId);
            }
            return false;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ChangePassword(PasswordModel model)
        {
            if (AppUser != null)
            {
                IdentityResult result = await MyService.MyUserManager.ChangePassword(AppUser.UserName, model.oldPassword, model.newPassword);
                if (result.IsSuccessful)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Forbidden);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// toggle following the target user of the current user user
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("following")]
        public bool ToggleFollow(Guid targetUserId)
        {
            if (AppUser != null)
            {
                return MyService.ToggoleFollow(targetUserId, AppUser.Id);
            }
            return false;
        }

        /// <summary>
        /// get all followee ids for the current user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("following")]
        public IEnumerable<Guid> GetAllFollowingIds()
        {
            if (AppUser != null)
            {
                return MyService.GetAllFollowingIds(AppUser.Id);
            }
            return null;
        }

        [HttpPost]
        public void ToggleRole(RoleModel model)
        {
            if (AppUser != null && AppUser.Roles.Contains("Admin"))
            {
                MyService.ToggoleUserRole(new Guid(model.id), model.role);
            }
        }
        #endregion

    }
    public class RoleModel
    {
        public string id { get; set; }
        public string role { get; set; }
    }

    public class PasswordModel
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}