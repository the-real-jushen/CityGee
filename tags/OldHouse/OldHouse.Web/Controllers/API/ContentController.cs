using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Jtext103.ImageHandler;
using OldHouse.Web.Models;

namespace OldHouse.Web.Controllers
{
    /// <summary>
    /// handles the stiatic content of the site
    /// </summary>
    public class ContentController : ApiController
    {
        /// <summary>
        /// upload a image to be a checkin photo,
        /// the photo is resized first and then croped from the center,
        /// the result will be a image @920*720px
        /// </summary>
        /// <returns> Ok(new UploadResponse { UploadedFileUrls = new List string {path}})</returns>
        [Authorize]
        [HttpPost]
        public IHttpActionResult UploadCheckInImage()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count != 1)
            {
                return Ok(new {error="上传失败"});
            }
            var postedFile = httpRequest.Files[0];
            string imageName = Guid.NewGuid().ToString();//生成图像名称
            string path = "/content/Images/checkins/" + imageName + postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("."));//相对路径+图像名称+图像格式
            string filePath = HttpContext.Current.Server.MapPath("~" + path);//绝对路径

            Stream fileStream = postedFile.InputStream;
            HandleImageService.CutForCustom(fileStream, filePath, 960, 720, 75);//剪裁为960*720并保存图像到本地
            fileStream.Close();

            return Ok(new UploadResponse { UploadedFileUrls = new List<string> {path}});
        }
    }
}
