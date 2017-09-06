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

namespace Jtext103.OldHouse.Business.Services
{
    public partial class  HouseService:EntityService<House>
    {
        public BlogPostService CheckInService;
        public EntityService<OldHouseUserProfile> ProfileService;

        public UserManager<OldHouseUser> MyUserManager { get; set; }


        public HouseService(IRepository<House> repository,BlogPostService checkinService,EntityService<OldHouseUserProfile> profileSerice, UserManager<OldHouseUser> userManger): base(repository)
        {
            CheckInService = checkinService;
            CheckInService.RegisterField<CheckIn>(new List<string>());
            ProfileService = profileSerice;
            //remember to register user private filds here
            MyUserManager = userManger;
            registerHouse();
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







        private  void registerHouse()
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
               
                var lng=double.Parse(location.Substring(0,location.IndexOf(";")));
                var lat= double.Parse(  location.Substring(location.IndexOf(";")+1));
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
        /// <retu
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
