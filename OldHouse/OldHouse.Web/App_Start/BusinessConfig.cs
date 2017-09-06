using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Jtext103.Repository;
using Jtext103.Repository.Interface;
using Jtext103.MongoDBProvider;
using Jtext103.OldHouse.Business.Models;
using Jtext103.OldHouse.Business.Services;
using AutoMapper;
using Jtext103.BlogSystem;
using Jtext103.EntityModel;
using Jtext103.Identity.Service;
using OldHouse.Web.Models;
using Jtext103.Volunteer.VolunteerMessage;
using Jtext103.Volunteer.VolunteerEvent;

namespace OldHouse.Web.App_Start
{
    /// <summary>
    /// config all biusiness here, like db and data mapping
    /// </summary>
    public class BusinessConfig
    {
        public static HouseService MyHouseService { get; set; }
        public static FeedbackService MyFeedbackService { get; set; }
        public static ArticleService MyArticleService { get; set; }
        /// <summary>
        /// config the business logic
        /// </summary>
        public static void ConfigBusiness()
        {
            //todo ioc is really needed here
            string database = "OldHouseDb";//"OldHouseDbProduction"
            MongoDBRepository<House> houseDb = new MongoDBRepository<House>(@"mongodb://127.0.0.1:27017", database, "House");
            MongoDBRepository<BlogPostEntity> CheckInDb = new MongoDBRepository<BlogPostEntity>(@"mongodb://127.0.0.1:27017", database, "CheckIn");
            MongoDBRepository<LikeRateFavEntity> LrfDb = new MongoDBRepository<LikeRateFavEntity>(@"mongodb://127.0.0.1:27017", database, "LikeRateFavorite");
            MongoDBRepository<OldHouseUser> UserDb = new MongoDBRepository<OldHouseUser>(@"mongodb://127.0.0.1:27017", database, "OldHouseUser");
            MongoDBRepository<OldHouseUserProfile> ProfileDb = new MongoDBRepository<OldHouseUserProfile>(@"mongodb://127.0.0.1:27017", database, "OldHouseUserProfile");
            MongoDBRepository<Message> feedDb = new MongoDBRepository<Message>(@"mongodb://127.0.0.1:27017", database, "Feed");

            MongoDBRepository<FeedBackEntity> FeedbackDb = new MongoDBRepository<FeedBackEntity>(@"mongodb://127.0.0.1:27017", database, "Feedback");
            MongoDBRepository<Article> ArticleDb = new MongoDBRepository<Article>(@"mongodb://127.0.0.1:27017", database, "Article");

            MongoDBRepository<Event> eventDb = new MongoDBRepository<Event>(@"mongodb://127.0.0.1:27017", database, "Event");
            MongoDBRepository<Subscriber> subscriberDb = new MongoDBRepository<Subscriber>(@"mongodb://127.0.0.1:27017", database, "Subscriber");
            //string handlerRelativePath = System.AppDomain.CurrentDomain.BaseDirectory.Substring(0, System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\').LastIndexOf(@"\")) + @"\OldHouse.EventHandlers\bin\Debug";
            //EventService.InitService(eventDb, subscriberDb, 100, 1000, handlerRelativePath);
            EventService.InitService(eventDb, subscriberDb, 100, 1000, HttpRuntime.BinDirectory + @"\..\OldHouseEventHandler");

            //a like rate fav service shared by all entity service
            var shareedLrfService = new LikeRateFavService(LrfDb);
            MyFeedbackService = new FeedbackService(FeedbackDb);
            MyArticleService = new ArticleService(ArticleDb);
            MyHouseService = new HouseService(
                houseDb,
                new BlogPostService(CheckInDb, shareedLrfService), 
                new EntityService<OldHouseUserProfile>(ProfileDb),
                shareedLrfService,
                new UserManager<OldHouseUser>(UserDb),
                new MessageService(feedDb)
                );
            ConfigDtoMapping();

            hotFix();
        }

        /// <summary>
        /// map DTO to bussiness model and viseversa
        /// </summary>
        public static void ConfigDtoMapping()
        {
            //house related mapping
            Mapper.CreateMap<House, HouseBrief>()
                .ForMember(dest => dest.CheckinCount,
                    opt => opt.MapFrom(src => MyHouseService.GetCheckInCountFor(src.Id)))
                .ForMember(dest => dest.LikeCount,
                    opt => opt.MapFrom(src => MyHouseService.LrfService.GetLRFCount(src.Id, LRFType.Like)))
                .ForMember(dest => dest.OwnerName,
                    opt =>
                    {
                        opt.MapFrom(src => MyHouseService.MyUserManager.FindByIdAsync(src.OwnerId).Result.NickName);
                        opt.NullSubstitute("小武");
                    });
            Mapper.CreateMap<House, HouseDetail>()
                .ForMember(dest => dest.BuiltYear,
                    opt => opt.ResolveUsing<YearResolver>())
                .ForMember(dest => dest.OwnerName,
                    opt => 
                    {
                        opt.MapFrom(src => MyHouseService.MyUserManager.FindByIdAsync(src.OwnerId).Result.NickName);
                        opt.NullSubstitute("小武");
                    })
                .ForMember(dest => dest.OwnerAvatar,
                    opt =>
                    {
                        opt.MapFrom(src => MyHouseService.MyUserManager.FindByIdAsync(src.OwnerId).Result.Avatar);
                        opt.NullSubstitute("");
                    });
            //display a oldhouse(maybe to edit it)
            Mapper.CreateMap<House, HouseDiscoverDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.OwnerId,
                    opt => opt.MapFrom(src => src.OwnerId.ToString()))
                .ForMember(dest => dest.IsApproved,
                    opt => opt.MapFrom(src => src.IsApproved.ToString()))
                .ForMember(dest => dest.BuiltYear,
                    opt => opt.MapFrom(src => ((DateTime)src.ExtraInformation["houseinfo-buildyear"]).Year))
                .ForMember(dest => dest.Lnt,
                    opt => opt.MapFrom(src => src.Location.coordinates[0]))
                .ForMember(dest => dest.Lat,
                    opt => opt.MapFrom(src => src.Location.coordinates[1]))
                .ForMember(dest => dest.Tags,
                    opt => opt.MapFrom(src => (src.Tags == null ? string.Empty : string.Join(",", src.Tags.ToArray()))));

            //create new oldHouse
            Mapper.CreateMap<HouseDiscoverDto, House>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => new Guid(src.Id)))
                .ForMember(dest => dest.OwnerId,
                    opt => opt.MapFrom(src => new Guid(src.OwnerId)))
                .ForMember(dest => dest.IsApproved,
                    opt => opt.MapFrom(src => Convert.ToBoolean(src.IsApproved)))
                .ForMember(dest => dest.Location,
                    opt => opt.MapFrom(src => HouseService.GetGeoPoint(src.Lnt + @";" + src.Lat)))
                .ForMember(dest => dest.ExtraInformation,
                    opt => opt.ResolveUsing<HouseExtraInformationResolver>())
                .ForMember(dest => dest.Tags,
                    opt => opt.MapFrom(src => (src.Tags == null ? new List<string>() : src.Tags.Split(new string[] { "," }, 100, StringSplitOptions.RemoveEmptyEntries).ToList<string>())));

            //all the checkin is regarded to be gcj-02 coords, but the current location may be using WGS84 coords,if there are notable drift,
            //we will transform the cuurent location coords to GJC-02
            //and we now assume the coords of the houses are GCJ-02, if not we will transfor there house coords to GCJ-02 coords
            //map New CheckinDTO to a real checkin object
            //child map
            Mapper.CreateMap<Asset,string>().ConvertUsing(src=>src.Path);
            Mapper.CreateMap<string, Asset>()
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Asset.IMAGE));  //the defual mapping is to image you can chage this later 

            //map checkin into checkin dto
            Mapper.CreateMap<CheckIn, CheckInDto>()
                .ForMember(dest => dest.Lat, opt => opt.MapFrom(src => src.Location.coordinates[1]))
                .ForMember(dest => dest.Lnt, opt => opt.MapFrom(src => src.Location.coordinates[0]))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Asset))
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => MyHouseService.MyUserManager.FindByIdAsync(src.User.Id).Result.Avatar))
                .ForMember(dest => dest.UserNickName, opt => opt.MapFrom(src => MyHouseService.MyUserManager.FindByIdAsync(src.User.Id).Result.NickName))
                .ForMember(dest => dest.ModifyTime, opt => opt.MapFrom(src => src.ModifyTime.ToShortDateString()))
                .ForMember(dest => dest.HouseName, opt => opt.MapFrom(src => MyHouseService.FindOneById(src.TargetId).Name))
                .ForMember(dest => dest.LocationString, opt => opt.MapFrom(src => MyHouseService.FindOneById(src.TargetId).LocationString))
                .ForMember(dest => dest.HouseCover, opt => opt.MapFrom(src => MyHouseService.FindOneById(src.TargetId).Cover));

            //0 need be replaced by read code
            //MyHouseService.ProfileService.FindOneById((Guid)src.Profiles[OldHouseUserProfile.PROFILENBAME]).FollowerCount)
            Mapper.CreateMap<OldHouseUser, UserInformationDto>()
                .ForMember(dest => dest.Who, opt => opt.ResolveUsing<WhoResolver>())
                .ForMember(dest => dest.Sex, opt => opt.ResolveUsing<SexResolver>())
                .ForMember(dest => dest.MyPoint, opt => opt.MapFrom(src => MyHouseService.GetProfile(src.Profiles[OldHouseUserProfile.PROFILENBAME]).Point))
                .ForMember(dest => dest.MyLevel, opt => opt.MapFrom(src => MyHouseService.GetProfile(src.Profiles[OldHouseUserProfile.PROFILENBAME]).Level))
                .ForMember(dest => dest.BeFollowedCount, opt => opt.MapFrom(src => MyHouseService.GetProfile(src.Profiles[OldHouseUserProfile.PROFILENBAME]).FollowerCount))
                .ForMember(dest => dest.BeLikedCount, opt => opt.MapFrom(src => MyHouseService.FindHouseLikedCountByUser(src.Id)+MyHouseService.FindLikedHouseCountByUser(src.Id)))
                .ForMember(dest => dest.DiscoveryCount, opt => opt.MapFrom(src => MyHouseService.FindHouseDiscoveryByUserCount(src.Id)))
                .ForMember(dest => dest.GrantedDiscoveryCount, opt => opt.MapFrom(src => MyHouseService.FindGrantedHouseByUserCount(src.Id)))
                .ForMember(dest => dest.CheckinCount, opt => opt.MapFrom(src => MyHouseService.FindChenkInCountByUser(src.Id)))
                .ForMember(dest => dest.GrantedCheckinCount, opt => opt.MapFrom(src => MyHouseService.FindGrantedChenkInCountByUser(src.Id)))
                .ForMember(dest => dest.FollowCount, opt => opt.MapFrom(src => MyHouseService.GetAllFollowingUserCount(src.Id)))
                .ForMember(dest => dest.LikedHouseCount, opt => opt.MapFrom(src => MyHouseService.FindLikedHouseCountByUser(src.Id)))
                .ForMember(dest => dest.LikedCheckinCount, opt => opt.MapFrom(src => MyHouseService.CheckInService.FindLikedBlogPostCountByUser(src.Id)))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.Roles.Contains("Admin")))
                .ForMember(dest => dest.IsEditor, opt => opt.MapFrom(src => src.Roles.Contains("Editor")))
                .ForMember(dest => dest.IsDeveloper, opt => opt.MapFrom(src => src.Roles.Contains("Developer")));
            Mapper.CreateMap<OldHouseUser, UserDisplayDto>()
                .ForMember(dest => dest.Who, opt => opt.ResolveUsing<WhoResolver>())
                .ForMember(dest => dest.Sex, opt => opt.ResolveUsing<SexResolver>())
                .ForMember(dest => dest.DiscoveryCount, opt => opt.MapFrom(src => MyHouseService.FindHouseDiscoveryByUserCount(src.Id)))
                .ForMember(dest => dest.CheckinCount, opt => opt.MapFrom(src => MyHouseService.FindChenkInCountByUser(src.Id)))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.Roles.Contains("Admin")))
                .ForMember(dest => dest.IsEditor, opt => opt.MapFrom(src => src.Roles.Contains("Editor")))
                .ForMember(dest => dest.IsDeveloper, opt => opt.MapFrom(src => src.Roles.Contains("Developer")))
                .ForMember(dest => dest.ProfileProvince, opt => opt.MapFrom(src => MyHouseService.GetCurrentProvinceFormProfile(src.Profiles[OldHouseUserProfile.PROFILENBAME])))
                .ForMember(dest => dest.ProfileCity, opt => opt.MapFrom(src => MyHouseService.GetCurrentCityFormProfile(src.Profiles[OldHouseUserProfile.PROFILENBAME])));

            //register user model to real user
            Mapper.CreateMap<RegisterViewModel, OldHouseUser>()
                .ForMember(dest=>dest.PasswordHash,opt=>opt.MapFrom(src=>src.Password));

            //feedback
            Mapper.CreateMap<FeedBackEntity, FeedBackDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => MyHouseService.MyUserManager.FindByNameAsync(src.UserName).Result.Id))
                .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => MyHouseService.MyUserManager.FindByNameAsync(src.UserName).Result.NickName))
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => MyHouseService.MyUserManager.FindByNameAsync(src.UserName).Result.Avatar));

            //Feed
            Mapper.CreateMap<Message, FeedDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Time.ToLocalTime().ToShortDateString()))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time.ToLocalTime().ToLongTimeString()))
                .ForMember(dest => dest.MessageFrom, opt => opt.ResolveUsing<UserResolver>())
                .ForMember(dest => dest.Type, opt => opt.ResolveUsing<FeedTypeResolver>());

            //Article
            Mapper.CreateMap<Article, ArticleBrief>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => MyHouseService.MyUserManager.FindByIdAsync(src.AuthorId).Result.NickName));
        }

        /// <summary>
        /// fisx the data base migration problem or something here
        /// remember, once you fixed the issue comment out the code!!!
        /// </summary>
        private static void hotFix()
        {
            //todo add UserId field in the profile
            //uncomment this is you have a old db that the profile dont have a userid field
            #region fix profile user id and follow
            //var profileIds = MyHouseService.ProfileService.EntityRepository.FindAll().Select(p => p.Id);
            //foreach (var pid in profileIds)
            //{
            //    var qDict = new Dictionary<string, object> {{"Profiles.OldHouseUserProfile", pid}};
            //    QueryObject<OldHouseUser> query=new QueryObject<OldHouseUser>(MyHouseService.MyUserManager.UserRepository);
            //    //query.AppendQuery(qDict, QueryLogic.And);
            //    query.AppendQuery(QueryOperator.Equal, "Profiles.OldHouseUserProfile", pid, QueryLogic.And);
            //    OldHouseUser tUser = null;
            //    try
            //    {
            //        tUser = MyHouseService.MyUserManager.UserRepository.Find(query).Single();
            //        ;                //var tUser=MyHouseService.MyUserManager.UserRepository.Find("Profiles.OldHouseUserProfile", pid).Single();
            //        var tProfile = MyHouseService.ProfileService.FindOneById(pid);
            //        tProfile.UserId = tUser.Id;
            //        MyHouseService.ProfileService.SaveOne(tProfile);
            //    }
            //    catch 
            //    {
            //        //var ii=tUser.Id;

            //    }

            //}
            #endregion

            #region fix old user that has no profile
            //var userIds = MyHouseService.MyUserManager.UserRepository.FindAll().Select(p => p.Id);
            //foreach (var uid in userIds)
            //{
                
            //    var tUser = MyHouseService.MyUserManager.FindByIdAsync(uid).Result;
            //    if (tUser.Profiles.ContainsKey("OldHouseUserProfile") != true ||
            //        tUser.Profiles["OldHouseUserProfile"] == Guid.Empty)
            //    {
            //        var nProfile = new OldHouseUserProfile{UserId = tUser.Id};
            //        MyHouseService.ProfileService.SaveOne(nProfile);
            //        tUser.Profiles.Add("OldHouseUserProfile", nProfile.Id);
            //        MyHouseService.MyUserManager.UpdateAsync(tUser).Wait();
            //    }

            //}

            #endregion

            #region add a system user to the old db

            //var result=MyHouseService.CreateUserWithProfile(
            //    new OldHouseUser {UserName = "SystemUser", NickName = "System", Id = MyHouseService.SystemUserId,Roles = new HashSet<string>{"System"},PasswordHash = "jfguHY957Uhlu"},
            //    new HashSet<string> {OldHouseUserProfile.PROFILENBAME}).Result;
            //result=null;

            #endregion

            #region fix userProfile's followerCount
            ////profile id及算出对应的followerCount的键值对
            //Dictionary<Guid, int> profileIdAndFollowerCount = new Dictionary<Guid, int>();
            //var allProfileIds = MyHouseService.ProfileService.EntityRepository.FindAll().Select(p => p.Id);
            //var allProfiles = MyHouseService.ProfileService.EntityRepository.FindAll();
            ////重新计算followerCount
            //foreach (Guid profileId in allProfileIds)
            //{
            //    int count = 0;
            //    foreach (var profile in allProfiles)
            //    {
            //        if (profile.FollowingIds.Contains(profileId))
            //        {
            //            count++;
            //        }
            //    }
            //    profileIdAndFollowerCount.Add(profileId, count);
            //}
            ////更新profile
            //foreach (Guid profileId in profileIdAndFollowerCount.Keys)
            //{
            //    var profile = MyHouseService.ProfileService.FindOneById(profileId);
            //    profile.FollowerCount = profileIdAndFollowerCount[profileId];
            //    MyHouseService.ProfileService.SaveOne(profile);
            //}
            #endregion

            #region fix house's cover for no cover house
            //var allHouse1 = MyHouseService.EntityRepository.FindAll();
            //foreach (var house in allHouse1)
            //{
            //    if (house.Cover == null || house.Cover == "")
            //    {
            //        house.Cover = @"/Content/Images/house/defaultCover.jpg";
            //        MyHouseService.EntityRepository.SaveOne(house);
            //    }
            //}
            #endregion

            #region fix house property 'Condition' 'BuiltYear' 'HistoricValue' 'PhotoValue' to ExtraInformation
            //var allHouse2 = MyHouseService.EntityRepository.FindAll();
            //foreach(var house in allHouse2)
            //{
            //    bool isModified = false;
            //    if (house.Condition != null)
            //    {
            //        house.ModifyExtraInformation("houseinfo-condition", house.Condition);
            //        house.Condition = null;
            //        isModified = true;
            //    }
            //    if (house.BuiltYear != null || house.ExtraInformation.ContainsKey("houseinfo-buildyear"))
            //    {
            //        house.ModifyExtraInformation("houseinfo-buildyear", house.BuiltYear);
            //        house.Condition = null;
            //        isModified = true;
            //    }
            //    //HistoricValue和PhotoValue本身就没用过，不需要修复了
            //    if (isModified == true)
            //    {
            //        MyHouseService.EntityRepository.SaveOne(house);
            //    }
            //}
            #endregion

            #region fix house city name, add "市"
            //var allHouse3 = MyHouseService.EntityRepository.FindAll();
            //foreach (var house in allHouse3)
            //{
            //    //city名最后不是“市”的，加上“市”
            //    if (house.City.Last() != '市')
            //    {
            //        house.City += '市';
            //        MyHouseService.EntityRepository.SaveOne(house);
            //    }
            //}
            #endregion

            #region fix feedback that has no CreatedTime
            //var allFeedBack = MyFeedbackService.EntityRepository.FindAll();
            //foreach(var feedback in allFeedBack)
            //{
            //    if (feedback.CreatedTime == null || feedback.CreatedTime.Equals(new DateTime()))
            //    {
            //        feedback.CreatedTime = DateTime.Now;
            //        MyFeedbackService.EntityRepository.SaveOne(feedback);
            //    }
            //}
            #endregion

            #region fix event that has no Time
            var allEvent = EventService.eventRepository.FindAll();
            foreach (var thisEvent in allEvent)
            {
                if (thisEvent.Time == null || thisEvent.Time.Equals(new DateTime()))
                {
                    thisEvent.Time = new DateTime(2015, 8, 1);
                    EventService.eventRepository.SaveOne(thisEvent);
                }
            }
            #endregion
        }

        public class HouseExtraInformationResolver : ValueResolver<HouseDiscoverDto, Dictionary<string, object>>
        {
            protected override Dictionary<string, object> ResolveCore(HouseDiscoverDto source)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("houseinfo-buildyear", new DateTime(source.BuiltYear, 10, 1));
                return result;
            }
        }
        public class WhoResolver : ValueResolver<OldHouseUser, string>
        {
            protected override string ResolveCore(OldHouseUser source)
            {
                string resutlt;
                switch (source.sex.ToLower())
                {
                    case "male":
                        resutlt = "他";
                        break;
                    case "female":
                        resutlt = "她";
                        break;
                    default:
                        resutlt = "Ta";
                        break;
                }
                return resutlt;
            }
        }
        public class SexResolver : ValueResolver<OldHouseUser, string>
        {
            protected override string ResolveCore(OldHouseUser source)
            {
                string resutlt;
                switch(source.sex.ToLower())
                {
                    case "male":
                        resutlt = "男";
                        break;
                    case "female":
                        resutlt = "女";
                        break;
                    default:
                        resutlt = "不明";
                        break;
                }
                return resutlt;
            }
        }
        public class YearResolver : ValueResolver<House, string>
        {
            protected override string ResolveCore(House source)
            {
                string resutlt;
                if (source.ExtraInformation.ContainsKey("houseinfo-buildyear"))
                {
                    int buildyear = ((DateTime)source.ExtraInformation["houseinfo-buildyear"]).Year;
                    if (buildyear == 1)
                    {
                        resutlt = "未知";
                    }
                    else if (buildyear == 1000)
                    {
                        resutlt = "1840年以前";
                    }
                    else
                    {
                        resutlt = buildyear + "年";
                    }
                }
                else
                {
                    resutlt = "未知";
                }
                return resutlt;
            }
        }
        public class UserResolver : ValueResolver<Message, OldHouseUser>
        {
            protected override OldHouseUser ResolveCore(Message source)
            {
                OldHouseUser resutlt;
                switch(source.MessageFrom.ToLower())
                {
                    case "system":
                        resutlt = null;
                        break;
                    default:
                        resutlt = MyHouseService.MyUserManager.FindByIdAsync(new Guid(source.MessageFrom)).Result;
                        break;
                }
                return resutlt;
            }
        }

        public class FeedTypeResolver : ValueResolver<Message, string>
        {
            protected override string ResolveCore(Message source)
            {
                if (source.Title.Equals("城迹新闻"))
                {
                    return "news";
                }
                else if(source.IsBroadcast)
                {
                    return "broadcast";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}