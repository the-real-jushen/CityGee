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
using Jtext103.Auth;
using Jtext103.Identity.Models;
using Jtext103.OldHouse.Business.Models;
using Jtext103.OldHouse.Business.Services;
using OldHouse.Web.App_Start;
using AutoMapper;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace OldHouse.Web.Controllers
{
    /// <summary>
    /// handles the stiatic content of the site
    /// </summary>
    public class ContentController : ApiController
    {
        public HouseService MyService;
        public ContentController()
        {
            MyService = BusinessConfig.MyHouseService;
        }
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
        /// <summary>
        /// upload a image to be a house photo,
        /// the photo is resized first and then croped from the center,
        /// the result will be a image @920*720px
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IHttpActionResult UploadHouseImage()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count != 1)
            {
                return Ok(new { error = "上传失败" });
            }
            var postedFile = httpRequest.Files[0];
            string imageName = Guid.NewGuid().ToString();//生成图像名称
            string path = "/content/Images/house/" + imageName + postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("."));//相对路径+图像名称+图像格式
            string filePath = HttpContext.Current.Server.MapPath("~" + path);//绝对路径

            Stream fileStream = postedFile.InputStream;
            HandleImageService.CutForCustom(fileStream, filePath, 960, 720, 75);//剪裁为960*720并保存图像到本地
            fileStream.Close();

            return Ok(new UploadResponse { UploadedFileUrls = new List<string> { path } });
        }
        /// <summary>
        /// Editor Upload Feed Image
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IHttpActionResult UploadFeedImage()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count != 1)
            {
                return Ok(new { error = "上传失败" });
            }
            var postedFile = httpRequest.Files[0];
            string imageName = Guid.NewGuid().ToString();//生成图像名称
            string path = "/content/Images/feed/" + imageName + postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("."));//相对路径+图像名称+图像格式
            string filePath = HttpContext.Current.Server.MapPath("~" + path);//绝对路径

            Stream fileStream = postedFile.InputStream;
            HandleImageService.CutForCustom(fileStream, filePath, 960, 720, 75);//剪裁为960*720并保存图像到本地
            fileStream.Close();

            return Ok(new UploadResponse { UploadedFileUrls = new List<string> { path } });
        }
        /// <summary>
        /// 上传头像
        /// 如果头像过大，则默认裁剪为128*128
        /// </summary>
        /// <returns></returns>
        [ActionName("UploadAvatar")]
        [HttpPost]
        public HttpResponseMessage UploadAvatar()
        {
            Guid userId = new Guid(User.Identity.GetUserId());
            OldHouseUser user = MyService.MyUserManager.FindByIdAsync(userId).Result;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count != 1)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent("请求不合法", System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            var postedFile = httpRequest.Files[0];
            string path = user.Id.ToString() + postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("."));//相对路径+图像名称+图像格式
            string filePath = HttpContext.Current.Server.MapPath("~/Content/Images/avatar/" + path);//绝对路径

            string oldAvatarPath = HttpContext.Current.Server.MapPath("~/Content/Images/avatar/" + user.Avatar);//根据相应Profile获得原图像相对路径
            if (File.Exists(oldAvatarPath) && !oldAvatarPath.ToLower().Contains("default")) //如果原图像存在且不为系统默认头像，则删除
            {
                File.Delete(oldAvatarPath);
            }

            Stream fileStream = postedFile.InputStream;
            HandleImageService.CutForCustom(fileStream, filePath, 256, 256, 75);//剪裁为256*256并保存图像到本地
            fileStream.Close();

            user.Avatar = path;//将新图像相对路径保存到当前用户的相应Profile中
            MyService.MyUserManager.UserRepository.SaveOne(user);

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        /// <summary>
        /// 生成页面网址的二维码
        /// </summary>
        /// <param name="model">activity的id</param>
        /// <returns></returns>
        [ActionName("QRCode")]
        [HttpGet]
        public HttpResponseMessage QRCode(string url)
        {
            MemoryStream ms = new MemoryStream();
            int moduleSize = 12;//二维码大小
            QuietZoneModules quietZones = QuietZoneModules.Two;//空白区域
            ErrorCorrectionLevel ecl = ErrorCorrectionLevel.M;//误差校正水平

            QrEncoder qrEncoder = new QrEncoder(ecl);
            QrCode qrCode = qrEncoder.Encode(url);
            var render = new GraphicsRenderer(new FixedModuleSize(moduleSize, quietZones));
            render.WriteToStream(qrCode.Matrix, System.Drawing.Imaging.ImageFormat.Png, ms);

            byte[] buffer = ms.GetBuffer();
            string result = Convert.ToBase64String(buffer);
            StringWriter tw = new StringWriter();
            JsonSerializer jsonSerializer = new JsonSerializer();
            jsonSerializer.Serialize(tw, result, result.GetType());
            return new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted, Content = new StringContent(tw.ToString(), System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        /// <summary>
        /// 获取用户创建的old house
        /// </summary>
        /// <returns></returns>
        [ActionName("MyHouses")]
        [HttpGet]
        public HttpResponseMessage MyHouses(string id)
        {
            if (id == null || id == "")
            {
                id = User.Identity.GetUserId();
            }
            Guid userId = new Guid(id);
            List<House> result = MyService.FindHouseDiscoveryByUser(userId, 0, 0).ToList<House>();
            StringWriter tw = new StringWriter();
            JsonSerializer jsonSerializer = new JsonSerializer();
            jsonSerializer.Serialize(tw, result, result.GetType());
            string jsonString = tw.ToString();
            return new HttpResponseMessage { Content = new StringContent(jsonString, System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        /// <summary>
        /// 获取用户的签到
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("MyCheckins")]
        [HttpGet]
        public HttpResponseMessage MyCheckins(string id)
        {
            if (id == null || id == "")
            {
                id = User.Identity.GetUserId();
            }
            Guid userId = new Guid(id);
            var checkins = MyService.CheckInService.FindAllBlogPostForUser(userId, 0, 0).Cast<CheckIn>();
            var result = Mapper.Map<IEnumerable<CheckInDto>>(checkins);

            StringWriter tw = new StringWriter();
            JsonSerializer jsonSerializer = new JsonSerializer();
            jsonSerializer.Serialize(tw, result, result.GetType());
            string jsonString = tw.ToString();
            return new HttpResponseMessage { Content = new StringContent(jsonString, System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };
        }


        /// <summary>
        /// get the verion info of the latest android app
        /// </summary>
        /// <returns></returns>
        [ActionName("androidversion")]
        [HttpGet]
        public UpgradeInfo GetLatestAndoidApp()
        {
            string androidFolderPath = HttpRuntime.AppDomainAppPath + "content\\Mobile\\Android";
            var  infoJson=File.ReadAllText(androidFolderPath + "\\Latest\\changelog.txt");
            var info = JsonConvert.DeserializeObject<UpgradeInfo>(infoJson);
            return info;
        }
    }
}
