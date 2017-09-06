using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Jtext103.BlogSystem;
using Jtext103.BlogSystem.Extension;
using OldHouse.Web.Models;

namespace OldHouse.Web.Controllers.API
{
    public class EditorController : BaseApiController
    {
        /// <summary>
        /// 将house认证状态取反
        /// </summary>
        /// <param name="model"></param>
        [Authorize]
        [HttpPost]
        public void ToggleApproveHouse(string id)
        {
            if (AppUser.Roles.Contains("Editor"))
            {
                MyService.ToggoleHouseAuthentication(new Guid(id));
            }
        }

        /// <summary>
        /// 将签到精华状态取反
        /// </summary>
        /// <param name="model"></param>
        [Authorize]
        [HttpPost]
        public void ToggleEssenceCheckIn(string id)
        {
            if (AppUser.Roles.Contains("Editor"))
            {
                MyService.ToggoleCheckInEssence(new Guid(id));
            }
        }
    }
}