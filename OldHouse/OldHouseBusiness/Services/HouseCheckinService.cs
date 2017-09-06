using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Jtext103.Repository.Interface;
using Jtext103.Repository;
using Jtext103.OldHouse.Business.Models;
using Jtext103.EntityModel;
using Jtext103.BlogSystem;
using Jtext103.Volunteer.VolunteerEvent;

namespace Jtext103.OldHouse.Business.Services
{
    /// <summary>
    /// the code about checkin for house service 
    /// !!!Mainly works with the _checkInService which is a blog servie, this can be refactored into a new service if it gets to fat
    /// </summary>
    public partial class HouseService
    {
        #region house's checkin

        public void CheckInHouse(Guid housId, CheckIn checkIn)
        {
            //the house id can be used to modify some properties in the house entity
            checkIn.TargetId = housId;
            CheckInService.SaveOne(checkIn);
            //产生NewCheckIn事件
            EventService.Publish("NewCheckInEvent", checkIn.Id, checkIn.User.Id);
        }

        public void CheckInHouse(Guid housId, Guid userId, string content,List<Asset> asset,GeoPoint  location)
        {
            //map the application user to this basic user
            var user= new BasicUser(userId, "!!Some User Name", "!!Some Nick name");
            var checkin = new CheckIn(user, housId, "some checkin!!", content, asset, location);
            CheckInHouse(housId, checkin);
        }

        /// <summary>
        /// you can call into the check in service directly
        /// </summary>
        /// <param name="houseId"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public IEnumerable<CheckIn> ListCheckInsFor(Guid houseId, int page, int pagesize)
        {
            //List<BlogPostEntity> source = new List<BlogPostEntity>();
            //List<BlogPostEntity> result = new List<BlogPostEntity>();
            ////先找到精华
            //Dictionary<string, object> queryDict1 = new Dictionary<string, object>();
            //queryDict1.Add("TargetId", houseId);
            //queryDict1.Add("IsEssence", true);
            //QueryObject<BlogPostEntity> queryObject1 = new QueryObject<BlogPostEntity>(CheckInService.EntityRepository);
            //queryObject1.AppendQuery(queryDict1, QueryLogic.And);
            //IEnumerable<BlogPostEntity> resultEssence = CheckInService.EntityRepository.Find(queryObject1, "ModifyTime", false);
            //source.AddRange(resultEssence);
            ////再找到非精华
            //Dictionary<string, object> queryDict2 = new Dictionary<string, object>();
            //queryDict2.Add("TargetId", houseId);
            //queryDict2.Add("IsEssence", false);
            //QueryObject<BlogPostEntity> queryObject2 = new QueryObject<BlogPostEntity>(CheckInService.EntityRepository);
            //queryObject2.AppendQuery(queryDict2, QueryLogic.And);
            //IEnumerable<BlogPostEntity> resultNotEssence = CheckInService.EntityRepository.Find(queryObject2, "ModifyTime", false);
            //source.AddRange(resultNotEssence);
            ////进行分页
            //if (page == 0 || pagesize == 0)
            //{
            //    return source;
            //}
            //result = source.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            //return result;

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("TargetId", houseId);
            QueryObject<BlogPostEntity> queryObject = new QueryObject<BlogPostEntity>(CheckInService.EntityRepository);
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            List<KeyValuePair<string, bool>> sorts = new List<KeyValuePair<string, bool>>();
            sorts.Add(new KeyValuePair<string, bool>("IsEssence", false));
            sorts.Add(new KeyValuePair<string, bool>("ModifyTime", false));
            var result = CheckInService.EntityRepository.Find(queryObject, sorts, page, pagesize).Cast<CheckIn>();
            return result;
        }

        public int GetCheckInCountFor(Guid houseId)
        {
            return (int)CheckInService.FindAllBlogPostCountFor(houseId);
        }

        /// <summary>
        /// 管理员进行check in的 精华贴/取消精华
        /// </summary>
        /// <param name="checkInId"></param>
        public void ToggoleCheckInEssence(Guid checkInId)
        {
            CheckIn checkIn = (CheckIn)CheckInService.FindOneById(checkInId);
            checkIn.IsEssence = !checkIn.IsEssence;
            CheckInService.SaveOne(checkIn);
            if (checkIn.IsEssence == true)
            {
                //产生GrantCheckInEssenceEvent事件
                EventService.Publish("GrantCheckInEssenceEvent", checkIn.Id, "system");
            }
            else
            {
                //产生CancelCheckInEssenceEvent事件
                EventService.Publish("CancelCheckInEssenceEvent", checkIn.Id, "system");
            }
        }

        #endregion
        #region user's checkin
        /// <summary>
        /// 找到该用户所有的Chenkin
        /// </summary>
        /// <param name="ownerId">the dicoverer is the house owner</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<CheckIn> FindChenkInByUser(Guid userId, int pageIndex, int pageSize)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("User._id", userId);
            var queryObject = new QueryObject<BlogPostEntity>(CheckInService.EntityRepository);
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            var result = CheckInService.EntityRepository.Find(queryObject, "CreateTime", false, pageIndex, pageSize).Cast<CheckIn>();
            return result;
        }
        /// <summary>
        /// 找到该用户所有被置为精华的Chenkin
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<CheckIn> FindGrantedChenkInByUser(Guid userId, int pageIndex, int pageSize)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("User._id", userId);
            queryDict.Add("IsEssence", true);
            var queryObject = new QueryObject<BlogPostEntity>(CheckInService.EntityRepository);
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            var result = CheckInService.EntityRepository.Find(queryObject, "CreateTime", false, pageIndex, pageSize).Cast<CheckIn>();
            return result;
        }
        /// <summary>
        /// 找到该用户Chenkin的数目
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int FindChenkInCountByUser(Guid userId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("User._id", userId);
            var queryObject = new QueryObject<BlogPostEntity>(CheckInService.EntityRepository);
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            int result = (int)CheckInService.EntityRepository.FindCountOfResult(queryObject);
            return result;
        }
        /// <summary>
        /// 找到该用户被设置为精华Chenkin的数目
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int FindGrantedChenkInCountByUser(Guid userId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("User._id", userId);
            queryDict.Add("IsEssence", true);
            var queryObject = new QueryObject<BlogPostEntity>(CheckInService.EntityRepository);
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            int result = (int)CheckInService.EntityRepository.FindCountOfResult(queryObject);
            return result;
        }
        /// <summary>
        /// 找到该用户Chenkin被赞的数目
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public int FindCheckinLikedCountByUser(Guid userId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("User._id", userId);
            var queryObject = new QueryObject<BlogPostEntity>(CheckInService.EntityRepository);
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            var checkins = CheckInService.EntityRepository.FindAsQueryable(queryObject);
            int checkinsLikedCount = 0;
            foreach (var checkin in checkins)
            {
                checkinsLikedCount += LrfService.GetLRFCount(checkin.Id, LRFType.Like);
            }
            return checkinsLikedCount;
        }
        #endregion
    }
}
