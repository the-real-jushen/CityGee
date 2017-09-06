using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.Repository.Interface;
using Jtext103.Repository;
using Jtext103.OldHouse.Business.Models;
using Jtext103.EntityModel;
using Jtext103.BlogSystem;
using Jtext103.Identity.Interfaces;
using Jtext103.Identity.Service;
using Jtext103.Volunteer.Friend;
using Jtext103.Volunteer.VolunteerMessage;
using Jtext103.Volunteer.VolunteerEvent;

namespace Jtext103.OldHouse.Business.Services
{
    public partial class HouseService : EntityService<House>
    {
        private static HouseService instence; 
        
        /// <summary>
        /// on instance of the house service. make sure you have only one house service instance, 
        /// or at least make them using the save DBs. the instance is the newest one, no strict sigleton
        /// </summary>
        public static HouseService Instence
        {
            get { return instence; }
        }

        public BlogPostService CheckInService;
        public EntityService<OldHouseUserProfile> ProfileService;
        public LikeRateFavService LrfService;
        public FollowService<OldHouseUserProfile> FollowService;
        public MessageService FeedService;
        
        public UserManager<OldHouseUser> MyUserManager { get; set; }


        public HouseService(IRepository<House> repository,BlogPostService checkinService,EntityService<OldHouseUserProfile> profileSerice,
            LikeRateFavService lrfService, UserManager<OldHouseUser> userManger, MessageService feedService): base(repository)
        {
            CheckInService = checkinService;
            CheckInService.RegisterField<CheckIn>(new List<string>());
            ProfileService = profileSerice;
            //remember to register user private filds here
            MyUserManager = userManger;
            LrfService = lrfService;
            //use the profile repository
            FollowService=new FollowService<OldHouseUserProfile>(ProfileService.EntityRepository);
            FeedService = feedService;
            registerHouse();
            registerUserProfile();
            instence = this;
        }

        public override void SaveOne(House entity)
        {
            base.SaveOne(entity);            
        }

        public House FindHouseByCodeName(string codeName)
        {
            var query=getNewQueryObject();
            Dictionary<string,object> qDict=new Dictionary<string,object> ();
            qDict.Add("CodeName",codeName);

            query.AppendQuery(qDict,QueryLogic.And);
            return EntityRepository.Find(query).FirstOrDefault();
        }

        public bool HouseExistByCodeName(string codeName)
        {
            var query = getNewQueryObject();
            Dictionary<string, object> qDict = new Dictionary<string, object>();
            qDict.Add("CodeName", codeName);

            query.AppendQuery(qDict, QueryLogic.And);
            var count=EntityRepository.Find(query).Count();
            return (count > 0) ? true : false;
        }

        
        /// <summary>
        /// search the house using a filer
        /// </summary>
        /// <param name="filterSource">形式为：aaa+bbb+ccc+tags:ddd,eee,aaa,bbb,ccc is contained in name</param>
        /// <param name="sortByKey"></param>
        /// <param name="isAscending"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<House> Find(string filterSource, string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
          
            //过滤
            QueryObject<House> queryObject = generateHouseFilter(filterSource);
            
            IEnumerable<House> source = EntityRepository.Find(queryObject, sortByKey, isAscending, pageIndex, pageSize);

            return source;
        }

        /// <summary>
        /// list all the house with paging and sorting
        /// </summary>
        /// <param name="sortByKey">sort keyy</param>
        /// <param name="isAscending">order</param>
        /// <param name="pageIndex">page number, 0 not paging</param>
        /// <param name="pageSize">page size, 0 not paging</param>
        /// <returns></returns>
        public IEnumerable<House> Find( string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
           
            IEnumerable<House> source = EntityRepository.FindAll(sortByKey, isAscending, pageIndex, pageSize);

            return source;
        }

        /// <summary>
        /// list all the house with paging and order by distance from the coordinates you provide
        /// </summary>
        /// <param name="coordinates">the geographic point coordinates, longitude;latitude</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<House> FindNear(string coordinates, int pageIndex, int pageSize)
        {
            var query = this.getNewQueryObject();
            query.AppendQuery(QueryOperator.Near, "Location", coordinates, QueryLogic.And);
            return EntityRepository.Find(query, pageIndex, pageSize);
        }

        /// <summary>
        /// listall houses dicovered by a user
        /// todo will add ordering
        /// </summary>
        /// <param name="ownerId">the dicoverer is the house owner</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<House> FindHouseDiscoveryByUser(Guid ownerId, int pageIndex, int pageSize)
        {
            var query = this.getNewQueryObject();
            var dict = new Dictionary<string, object>
            {
                {"OwnerId",ownerId}
            };
            query.AppendQuery(dict,QueryLogic.And);
            return EntityRepository.Find(query, pageIndex, pageSize);
        }
        /// <summary>
        /// listall granted houses dicovered by a user
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<House> FindGrantedHouseByUser(Guid ownerId, int pageIndex, int pageSize)
        {
            var query = this.getNewQueryObject();
            var dict = new Dictionary<string, object>
            {
                {"OwnerId",ownerId},
                {"IsApproved",true}
            };
            query.AppendQuery(dict, QueryLogic.And);
            return EntityRepository.Find(query, pageIndex, pageSize);
        }
        /// <summary>
        /// 生成数据库查询query
        /// </summary>
        /// <param name="userQuery"></param>
        /// <returns></returns>
        private QueryObject<House> buildFilter(Dictionary<string, string> userQuery)
        {
            QueryObject<House> queryObject = new QueryObject<House>(EntityRepository);
            //精确查找city
            if (userQuery.ContainsKey("city"))
            {
                queryObject.AppendQuery(QueryOperator.Like, "City", userQuery["city"], QueryLogic.And);
            }
            //精确查找ownerId
            if (userQuery.ContainsKey("ownerid"))
            {
                queryObject.AppendQuery(QueryOperator.Equal, "OwnerId", new Guid(userQuery["ownerid"]), QueryLogic.And);
            }
            //地理位置查询
            if(userQuery.ContainsKey("location"))
            {
                queryObject.AppendQuery(QueryOperator.Near, "Location", userQuery["location"], QueryLogic.And);
            }
            //用户查找老房子的Name和Tag（用户的query用空格分隔）
            //模糊查找Name，精确查找Tag
            if (userQuery.ContainsKey("search"))
            {
                if (userQuery["search"] != "")
                {
                    QueryObject<House> userSearchQueryObject = new QueryObject<House>(EntityRepository);
                    string[] userSearch = userQuery["search"].Split(' ');
                    foreach (string query in userSearch)
                    {
                        //模糊查询Name
                        userSearchQueryObject.AppendQuery(QueryOperator.Like, "Name", query, QueryLogic.Or);
                        //精确查询Tag
                        Dictionary<string, object> queryDict = new Dictionary<string, object>();
                        queryDict.Add("Tags", query);
                        userSearchQueryObject.AppendQuery(queryDict, QueryLogic.Or);
                    }
                    queryObject.AppendQuery(userSearchQueryObject, QueryLogic.And);
                }
            }
            return queryObject;
        }
        
        /// <summary>
        /// 使用过滤搜索老房子
        /// </summary>
        /// <param name="userQuery"></param>
        /// <param name="sortByKey"></param>
        /// <param name="isAscending"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<House> FilterHouse(Dictionary<string, string> userQuery, string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
            QueryObject<House> query = buildFilter(userQuery);
            IEnumerable<House> result = EntityRepository.Find(query, sortByKey, isAscending, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// 过滤搜索老房子的结果数目
        /// </summary>
        /// <param name="userQuery"></param>
        /// <returns></returns>
        public long FilterHouseCount(Dictionary<string, string> userQuery)
        {
            QueryObject<House> query = buildFilter(userQuery);
            long result = EntityRepository.FindCountOfResult(query);
            return result;
        }
        /// <summary>
        /// 找到该用户发现的house数目
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public int FindHouseDiscoveryByUserCount(Guid ownerId)
        {
            var query = this.getNewQueryObject();
            var dict = new Dictionary<string, object>
            {
                {"OwnerId",ownerId}
            };
            query.AppendQuery(dict, QueryLogic.And);
            return (int)EntityRepository.FindCountOfResult(query);
        }
        /// <summary>
        /// 找到该用户发现的house被置为精华的数目
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public int FindGrantedHouseByUserCount(Guid ownerId)
        {
            var query = this.getNewQueryObject();
            var dict = new Dictionary<string, object>
            {
                {"OwnerId",ownerId},
                {"IsApproved",true}
            };
            query.AppendQuery(dict, QueryLogic.And);
            return (int)EntityRepository.FindCountOfResult(query);
        }
        /// <summary>
        /// 找到该用户发现house被赞的数目
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public int FindHouseLikedCountByUser(Guid ownerId)
        {
            var query = this.getNewQueryObject();
            var dict = new Dictionary<string, object>
            {
                {"OwnerId",ownerId}
            };
            query.AppendQuery(dict, QueryLogic.And);
            var houses = EntityRepository.FindAsQueryable(query);
            int housesLikedCount = 0;
            foreach (var house in houses)
            {
                housesLikedCount += LrfService.GetLRFCount(house.Id, LRFType.Like);
            }
            return housesLikedCount;
        }
        /// <summary>
        /// 找到该用户Like过的所有House
        /// </summary>
        /// <param name="ownerId">该用户id</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<House> FindLikedHouseByUser(Guid ownerId, int pageIndex, int pageSize)
        {
            var likedHouseIds = LrfService.GetAllMyLRFIds(ownerId, LRFType.Like);
            var query = this.getNewQueryObject();
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            Dictionary<string, object> subQueryDict = new Dictionary<string, object>();
            subQueryDict.Add("$in", likedHouseIds);
            queryDict.Add("_id", subQueryDict);
            query.AppendQuery(queryDict, QueryLogic.And);
            return EntityRepository.Find(query, pageIndex, pageSize);
        }

        /// <summary>
        /// 找到该用户Like过的所有House的数目
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public long FindLikedHouseCountByUser(Guid ownerId)
        {
            var likedHouseIds = LrfService.GetAllMyLRFIds(ownerId, LRFType.Like);
            var query = this.getNewQueryObject();
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            Dictionary<string, object> subQueryDict = new Dictionary<string, object>();
            subQueryDict.Add("$in", likedHouseIds);
            queryDict.Add("_id", subQueryDict);
            query.AppendQuery(queryDict, QueryLogic.And);
            return EntityRepository.FindCountOfResult(query);
        }

        /// <summary>
        /// 检查给定id是house还是checkIn
        /// </summary>
        /// <param name="id"></param>
        /// <returns>如果为house，则返回0；如果为checkIn，则返回1；如果都不是，则返回-1</returns>
        public int CheckTheIdIsHouseOrCheckin(Guid id)
        {
            House house = FindOneById(id);
            if (house != null)
            {
                return 0;
            }
            else
            {
                var checkIn = CheckInService.FindOneById(id);
                if (checkIn != null)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 管理员进行老房子的官方认证/取消认证
        /// </summary>
        /// <param name="houseId"></param>
        public void ToggoleHouseAuthentication(Guid houseId)
        {
            House house = FindOneById(houseId);
            house.IsApproved = !house.IsApproved;
            EntityRepository.SaveOne(house);
            if (house.IsApproved == true)
            {
                //产生GrantHouseAuthenticationEvent事件
                EventService.Publish("GrantHouseAuthenticationEvent", house.Id, "system");
            }
            else
            {
                //产生CancelHouseAuthenticationEvent事件
                EventService.Publish("CancelHouseAuthenticationEvent", house.Id, "system");
            }
        }

        private void registerHouse()
        {
            EntityRepository.RegisterMap<House>(new string[] { "_overallValue" });
        }



        #region helper
        /// <summary>
        /// get a GeoPoint Object, the location string is "longitude;latitude"
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static GeoPoint GetGeoPoint(string location)
        {
            try
            {
                var lng = double.Parse(location.Substring(0,location.IndexOf(";")));
                var lat = double.Parse(location.Substring(location.IndexOf(";")+1));
                return new GeoPoint(lng,lat);
            }
            catch
            {
                throw new System.Exception("invalide location");
            }
        }

        public static string GetDistanceKm(GeoPoint loc1, GeoPoint loc2)
        {
            var EARTH_RADIUS = 6378.137;

            var lng1 = loc1.coordinates[0];
            var lng2 = loc2.coordinates[0];
            var lat1 = loc1.coordinates[1];
            var lat2 = loc2.coordinates[1];
            double radLat1 = rad(lat1);   
            double radLat2 = rad(lat2);   
            double a = radLat1 - radLat2;   
            double b = rad(lng1) - rad(lng2);   
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a/2),2) +   
            Math.Cos(radLat1)*Math.Cos(radLat2)*Math.Pow(Math.Sin(b/2),2)));   
            s = s * EARTH_RADIUS;   
            s = Math.Round(s * 10000) / 10000;   
            return ((int)(s)).ToString();   
        }


        
        private static double rad(double d)   
        {   
             return d * Math.PI / 180.0;   
        }




        /// <summary>
        /// 用于生成查找activity的queryObject
        /// </summary>
        /// <param name="filterSource">形式为：aaa+bbb+ccc+tags:ddd,eee</param>
        /// <returns></returns>
        private QueryObject<House> generateHouseFilter(string filterSource)
        {
            QueryObject<House> queryObject = new QueryObject<House>(EntityRepository);
            //过滤filterSource
            if (filterSource != null & filterSource != "")
            {
                //从url中获取的filterSource
                //需要将"%3A"替换为":"
                //"%2C"替换为","
                string filterResult = filterSource.Replace("%3A", ":").Replace("%2C", ",");
                //用'+'分割，前半部分未被标记的为name，用tag:标记的为tag
                string[] strings = filterSource.Split('+');
                List<string> names = new List<string>();
                List<string> tags = new List<string>();
                //将filterSource中的filter信息读出来
                foreach (string str in strings)
                {
                    //tag
                    if (str.IndexOf("tags:") == 0)
                    {
                        string[] tagsString = str.Substring("tags:".Length).Split(',');
                        foreach (string tag in tagsString)
                        {
                            tags.Add(tag);
                        }
                    }
                    //name
                    else
                    {
                        names.Add(str);
                    }
                }
                //构建Name query
                //currentlly now suitable at leat not usful for house
                if (names.Any())
                {
                    QueryObject<House> nameQueryObject = new QueryObject<House>(EntityRepository);
                    foreach (string name in names)
                    {
                        nameQueryObject.AppendQuery(QueryOperator.Like, "Name", name, QueryLogic.Or);
                    }
                    queryObject.AppendQuery(nameQueryObject, QueryLogic.And);
                }
                //构建Tag query
                if (tags.Any())
                {
                    Dictionary<string, object> queryDict = new Dictionary<string, object>();
                    Dictionary<string, object> subQueryDict = new Dictionary<string, object>();
                    subQueryDict.Add("$all", tags);
                    queryDict.Add("Tags", subQueryDict);
                    queryObject.AppendQuery(queryDict, QueryLogic.And);
                }
            }
            return queryObject;
        }


        #endregion


        
    }
}
