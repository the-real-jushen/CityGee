using Jtext103.MongoDBProvider;
using Jtext103.OldHouse.Business.Models;
using Jtext103.Repository;
using Jtext103.Volunteer.VolunteerEvent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldHouseStatistics
{
    class Program
    {
        static MongoDBRepository<Event> EventDb;
        static MongoDBRepository<House> HouseDb;
        static MongoDBRepository<OldHouseUser> UserDb;

        static void Main(string[] args)
        {
            initRepository();

            StreamWriter sw = null;

            #region 统计用户浏览老房子
            QueryObject<Event> findAllViewAHouseEvent = new QueryObject<Event>(EventDb);
            findAllViewAHouseEvent.AppendQuery(QueryOperator.Equal, "Type", "ViewAHouseEvent", QueryLogic.And);
            var allViewAHouseEvent = EventDb.Find(findAllViewAHouseEvent);
            //用来统计每个用户共浏览了多少次,key为用户id
            Dictionary<Guid, NameAndCount> userViewHouse = new Dictionary<Guid, NameAndCount>();
            //用来统计每个老房子被浏览了多少次,key为house id
            Dictionary<Guid, NameAndCount> houseHasViewed = new Dictionary<Guid, NameAndCount>();
            foreach (var viewAHouseEvent in allViewAHouseEvent)
            {
                #region userViewHouse
                Guid userId = (Guid)viewAHouseEvent.Sender;
                if (userViewHouse.ContainsKey(userId))
                {
                    userViewHouse[userId].Count++;
                }
                else
                {
                    string userName;
                    if (userId == Guid.Empty)
                    {
                        userName = "匿名用户";
                    }
                    else
                    {
                        userName = UserDb.FindOneById(userId).NickName;
                    }
                    userViewHouse.Add(userId, new NameAndCount(userName, 1));
                }
                #endregion

                #region houseHasViewed
                Guid houseId = (Guid)viewAHouseEvent.Subject;
                if (houseHasViewed.ContainsKey(houseId))
                {
                    houseHasViewed[houseId].Count++;
                }
                else
                {
                    string houseName;
                    if (HouseDb.FindOneById(houseId) == null)
                    {
                        houseName = "未知";
                    }
                    else
                    {
                        houseName = HouseDb.FindOneById(houseId).Name;
                    }
                    houseHasViewed.Add(houseId, new NameAndCount(houseName, 1));
                }
                #endregion
            }            
            //统计用户浏览
            sw = new StreamWriter(@"C:\用户浏览老房子.csv", false, System.Text.Encoding.UTF8);
            sw.WriteLine("用户昵称,浏览次数");
            sw.WriteLine("总数," + allViewAHouseEvent.Count());
            foreach (KeyValuePair<Guid, NameAndCount> userViewHouseKeyValuePair in userViewHouse)
            {
                sw.WriteLine(userViewHouseKeyValuePair.Value.Name + "," + userViewHouseKeyValuePair.Value.Count);
            }
            sw.Dispose();
            //统计老房子被浏览
            sw = new StreamWriter(@"C:\老房子被浏览.csv", false, System.Text.Encoding.UTF8);
            sw.WriteLine("house名,浏览次数");
            sw.WriteLine("总数," + allViewAHouseEvent.Count());
            foreach (KeyValuePair<Guid, NameAndCount> houseHasViewedKeyValuePair in houseHasViewed)
            {
                sw.WriteLine(houseHasViewedKeyValuePair.Value.Name + "," + houseHasViewedKeyValuePair.Value.Count);
            }
            sw.Dispose();

            #endregion

            #region 统计用户like
            QueryObject<Event> findAllLikeEvent = new QueryObject<Event>(EventDb);
            findAllLikeEvent.AppendQuery(QueryOperator.Equal, "Type", "LikeEvent", QueryLogic.And);
            var allLikeEvent = EventDb.Find(findAllLikeEvent);
            //用来统计用户点赞次数
            Dictionary<Guid, NameAndCount> userLike = new Dictionary<Guid, NameAndCount>();
            foreach(var likeEvent in allLikeEvent)
            {
                Guid userId = (Guid)likeEvent.Sender;
                if(userLike.ContainsKey(userId))
                {
                    userLike[userId].Count++;
                }
                else
                {
                    string userName = UserDb.FindOneById(userId).NickName;
                    userLike.Add(userId, new NameAndCount(userName, 1));
                }
            }
            sw = new StreamWriter(@"C:\用户点赞次数.csv", false, System.Text.Encoding.UTF8);
            sw.WriteLine("用户昵称,点赞次数");
            sw.WriteLine("总数," + allLikeEvent.Count());
            foreach(KeyValuePair<Guid,NameAndCount> userLikeKeyValuePair in userLike)
            {
                sw.WriteLine(userLikeKeyValuePair.Value.Name + "," + userLikeKeyValuePair.Value.Count);
            }
            sw.Dispose();
            #endregion

        }

        static void initRepository()
        {
            Console.WriteLine("请选择要统计的数据库：(只能123，输错了就gg)");
            Console.WriteLine("1.localhost");
            Console.WriteLine("2.测试环境");
            Console.WriteLine("3.生产环境");
            char key = Console.ReadKey().KeyChar;
            string server;
            string database;
            switch (key)
            {
                case '1':
                    server = @"mongodb://127.0.0.1:27017";
                    database = "OldHouseDb";
                    break;
                case '2':
                    server = @"mongodb://115.156.252.5:27017";
                    database = "OldHouseDb";
                    break;
                case '3':
                    server = @"mongodb://115.156.252.5:27017";
                    database = "OldHouseDbProduction";
                    break;
                default:
                    throw new Exception();
            }
            EventDb = new MongoDBRepository<Event>(server, database, "Event");
            HouseDb = new MongoDBRepository<House>(server, database, "House");
            UserDb = new MongoDBRepository<OldHouseUser>(server, database, "OldHouseUser");

            //注册
            HouseDb.RegisterMap<House>(new string[] { "_overallValue" });
        }
    }

    class NameAndCount
    {
        public string Name { get; set; }
        public int Count { get; set; }

        public NameAndCount(string name, int count)
        {
            Name = name;
            Count = count;
        }
    }
}
