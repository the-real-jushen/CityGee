using AutoMapper;
using Jtext103.OldHouse.Business.Models;
using Jtext103.Volunteer.VolunteerMessage;
using OldHouse.Web.App_Start;
using OldHouse.Web.Controllers;
using OldHouse.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OldHouse.Web.Areas.Editor.Controllers
{
    /// <summary>
    /// 编辑工作站
    /// </summary>
    public class EditorController : BaseController
    {
        /// <summary>
        /// 获取编辑操作首页
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            if (!AppUser.Roles.Contains("Editor"))
            {
                Jtext103.Auth.Jtext103AuthMiddleware<Jtext103.OldHouse.Business.Models.OldHouseUser>.Logout(HttpContext.GetOwinContext().Environment);
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            return View();
        }
        /// <summary>
        /// 获取发布Feed页面
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult ReleaseFeed()
        {
            var NewFeed = new ReleaseFeedDto();
            return View(NewFeed);
        }
        /// <summary>
        /// 获取发布Article页面
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult ReleaseArticle()
        {
            var NewArticle = new ReleaseArticleDto();
            return View(NewArticle);
        }
        /// <summary>
        /// 处理发布Feed请求
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult ReleaseFeed(ReleaseFeedDto model)
        {
            if (!AppUser.Roles.Contains("Editor"))
            {
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                if(model.ReceiverEmail != null && MyService.MyUserManager.FindByNameAsync(model.ReceiverEmail).Result == null)
                {
                    ModelState.AddModelError("", "无此用户");
                    return View("ReleaseFeed", model);
                }
                var feed = new Message(){
                    MessageType = "Feed.Release",
                    IsBroadcast = model.IsBroadcast,
                    Title = model.FeedTitle,
                    Text = model.FeedText,
                    Pictures = (model.Images == null ? new List<string>() : model.Images),
                    DestinationLink = (model.DestinationLink.Equals("#") ? "javascript:void(0);" : model.DestinationLink),
                    NewBlank = (model.DestinationLink.Equals("#") ? false : model.NewBlank),
                    ReceiverId = ((model.ReceiverEmail == null || model.ReceiverEmail.Equals("")) ? Guid.Empty : MyService.MyUserManager.FindByNameAsync(model.ReceiverEmail).Result.Id),
                    MessageFrom = AppUser.Id.ToString()
                };
                MyService.FeedService.SendMessage(feed);
                ViewBag.Information = "Feed【"+model.FeedTitle+"】发布成功！";
            }
            return View("ReleaseFeed", model);
        }
        /// <summary>
        /// 处理发布/修改Article请求
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("ReleaseArticle")]
        public ActionResult AddOrModifyArticle(ReleaseArticleDto model)
        {
            if (!AppUser.Roles.Contains("Editor"))
            {
                return View("Error");
            }
            if (model.Id == null || model.Id.Equals(""))
            {
                model.Id = Guid.NewGuid().ToString();
            }
            if (ModelState.IsValid)
            {
                var article = BusinessConfig.MyArticleService.FindOneById(new Guid(model.Id));
                //发布新article时生成广播feed
                if (article == null)
                {
                    article = new Article()
                    {
                        AuthorId = AppUser.Id,
                        Title = model.ArticleTitle,
                        Body = model.ArticleBody
                    };
                    //是否发送广播feed
                    if(model.IsReleaseBroadcastFeed)
                    {
                        var feed = new Message()
                        {
                            MessageType = "Feed.Article",
                            IsBroadcast = true,
                            Title = "城迹新闻",
                            Text = model.ArticleTitle,
                            Pictures = new List<string>(),
                            DestinationLink = "/Editor/Article/" + article.Id.ToString(),
                            NewBlank = true,
                            ReceiverId = Guid.Empty,
                            MessageFrom = AppUser.Id.ToString()
                        };
                        MyService.FeedService.SendMessage(feed);
                    }
                }
                else
                {
                    article.Title = model.ArticleTitle;
                    article.Body = model.ArticleBody;
                }
                BusinessConfig.MyArticleService.SaveOne(article);
                return RedirectToAction("Article", new { id = article.Id.ToString() });
            }
            return View("ReleaseArticle", model);
        }
        /// <summary>
        /// 获取某Article的编辑页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [ActionName("EditArticle")]
        public ActionResult GetEditArticle(string id = "")
        {
            if(id.Equals(""))
            {
                return View("Error");
            }
            var article = BusinessConfig.MyArticleService.FindOneById(new Guid(id));
            if (article == null)
            {
                return View("Error");
            }
            var model = new ReleaseArticleDto()
            {
                Id = new Guid(id).ToString(),
                ArticleTitle = article.Title,
                ArticleBody = article.Body
            };
            return View("ReleaseArticle", model);
        }
        /// <summary>
        /// 获取Article详情页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("Article")]
        public ActionResult GetArticleDetail(string id = "")
        {
            if (id.Equals(""))
            {
                return View("Error");
            }
            var article = BusinessConfig.MyArticleService.FindOneById(new Guid(id));
            if (article == null)
            {
                return View("Error");
            }
            var author = MyService.MyUserManager.FindByIdAsync(article.AuthorId).Result;
            var model = new ArticleDetail()
            {
                Id = article.Id,
                AuthorId = article.AuthorId,
                AuthorAvatar = author.Avatar,
                AuthorNickNmae = author.NickName,
                CreatedTime = article.CreatedTime,
                Title = article.Title,
                Body = article.Body
            };
            if(AppUser != null && AppUser.Id.Equals(author.Id))
            {
                ViewBag.ShowEdit = true;
            }
            else
            {
                ViewBag.ShowEdit = false;
            }
            return View(model);
        }
        /// <summary>
        /// 获取Article列表页面
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ActionName("ArticleList")]
        public ActionResult GetArticleList(int page = 1, int pagesize = 6)
        {
            var lastpage = (int)Math.Ceiling(BusinessConfig.MyArticleService.FindAllArticleCount() / (double)pagesize);
            ViewBag.PageControl = new PageControl(page, lastpage, pagesize);
            ViewBag.Title = "城迹新闻";
            return View("ArticleList");
        }
        /// <summary>
        /// 获取Article列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ActionName("ArticleBriefList")]
        public ActionResult GetArticleBriefList(int page = 1, int pagesize = 6)
        {
            var articles = BusinessConfig.MyArticleService.FindAllArticle("CreatedTime", false, page, pagesize);
            var articlesBrief = Mapper.Map<IEnumerable<ArticleBrief>>(articles);
            return PartialView("_PartialArticleBriefList", articlesBrief);
        }
        /// <summary>
        /// 获取House管理页面
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="sortbykey"></param>
        /// <param name="ascend"></param>
        /// <returns></returns>
        [ActionName("HouseManagement")]
        public ActionResult HouseManagement(int page = 1, int pagesize = 6, string search = "", string sortbykey = "CreateTime", bool ascend = false)
        {
            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("search", search);
            var lastpage = (int)Math.Ceiling(MyService.FilterHouseCount(filter) / (double)pagesize);
            ViewBag.PageControl = new PageControl(page, lastpage, pagesize);
            return View();
        }
        /// <summary>
        /// 获取Checkin管理页面
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="search"></param>
        /// <param name="sortbykey"></param>
        /// <param name="ascend"></param>
        /// <returns></returns>
        [ActionName("CheckinManagement")]
        public ActionResult CheckinManagement(int page = 1, int pagesize = 6, string search = "", string sortbykey = "CreateTime", bool ascend = false)
        {
            return View();
        }
    }
}