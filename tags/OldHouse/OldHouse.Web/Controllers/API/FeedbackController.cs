using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using Jtext103.OldHouse.Business.Models;
using OldHouse.Web.App_Start;
using OldHouse.Web.Models;

namespace OldHouse.Web.Controllers.API
{
    public class FeedbackController : ApiController
    {

        /// <summary>
        /// insert a new feedback
        /// </summary>
        /// <param name="feedback"></param>
        [HttpPost]
        public void feedback(FeedBackEntity feedback)
        {
            BusinessConfig.MyFeedbackService.InsertOne(feedback);
        }
    }
}
