using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.Repository.Interface;
using Jtext103.Repository;
using Jtext103.OldHouse.Business.Models;
using Jtext103.EntityModel;
using Jtext103.BlogSystem;
using Jtext103.Identity.Models;

namespace Jtext103.OldHouse.Business.Services
{
    /// <summary>
    /// the code about user and profile logic for house service 
    /// this can be refactored into a new service if it gets to fat
    /// </summary>
    public partial class HouseService
    {
        #region profile
        /// <summary>
        /// create a fake and empty profile
        /// </summary>
        /// <returns>the id of the created profile</returns>
        public OldHouseUserProfile CreateAFakeProfile()
        { 
            var profile=new OldHouseUserProfile();
            profile.ProfileName = OldHouseUserProfile.PROFILENBAME;
            profile.ModifyExtraInformation("Test-Fake", "Fake!!!");
            ProfileService.SaveOne(profile);
            return profile;
        }

        /// <summary>
        /// create new profiles and saved into db, it the profile with the name is not supported thenthe profile is not created.
        /// </summary>
        /// <param name="profileNames">profile names, no duplocate</param>
        /// <returns></returns>
        public Dictionary<string, Guid> CreateNewProfile(HashSet<string> profileNames,Guid userId)
        {
            var dict = new Dictionary<string, Guid>();
            foreach (var profileName in profileNames)
            {
                var profile = GetNewProfileWithoutSaveToDb(profileName);
                if (profile != null)
                {
                    profile.UserId = userId;
                    ProfileService.SaveOne(profile);
                    dict.Add(profileName, profile.Id);
                }
            }
            return dict;
        }

        /// <summary>
        /// get a new profile inctance but not save to db, it he profile name is not supported
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public OldHouseUserProfile GetNewProfileWithoutSaveToDb(string profileName)
        {
            switch (profileName)
            {
                //the defualt user profile
                case OldHouseUserProfile.PROFILENBAME:
                    return new OldHouseUserProfile { ProfileName = profileName };
                default:
                    return null;
            }
        }

        public OldHouseUserProfile GetProfile(Guid profileId)
        {
            return ProfileService.FindOneById(profileId);
        }

        
        /// <summary>
        /// 添加或者修改profile中当前城市信息
        /// </summary>
        /// <param name="profileId">OldHouseUserProfile的id</param>
        /// <param name="city"></param>
        public void AddOrModifyCurrentCityForProfile(Guid profileId, string city)
        {
            var profile = ProfileService.EntityRepository.FindOneById(profileId);
            profile.ModifyExtraInformation("city-currentcity", city);
            ProfileService.EntityRepository.SaveOne(profile);
        }

        /// <summary>
        /// 从profile中获取当前城市信息
        /// 如果没有当前城市，则返回null
        /// </summary>
        /// <param name="profileId">OldHouseUserProfile的id</param>
        /// <returns></returns>
        public string GetCurrentCityFormProfile(Guid profileId)
        {
            var profile = ProfileService.EntityRepository.FindOneById(profileId);
            if (profile.ExtraInformation.ContainsKey("city-currentcity"))
            {
                return (string)profile.ExtraInformation["city-currentcity"];
            }
            else
            {
                return "武汉市";
            }
        }

        /// <summary>
        /// 添加或者修改profile中当前省份信息
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="province"></param>
        public void AddOrModifyCurrentProvinceForProfile(Guid profileId, string province)
        {
            var profile = ProfileService.EntityRepository.FindOneById(profileId);
            profile.ModifyExtraInformation("city-currentprovince", province);
            ProfileService.EntityRepository.SaveOne(profile);
        }

        /// <summary>
        /// 从profile中获取当前省份信息
        /// 如果没有当前城市，则返回null
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public string GetCurrentProvinceFormProfile(Guid profileId)
        {
            var profile = ProfileService.EntityRepository.FindOneById(profileId);
            if (profile.ExtraInformation.ContainsKey("city-currentprovince"))
            {
                return (string)profile.ExtraInformation["city-currentprovince"];
            }
            else
            {
                return "湖北省";
            }
        }

        /// <summary>
        /// 注册userProfile
        /// </summary>
        public void registerUserProfile()
        {
            ProfileService.EntityRepository.RegisterMap<OldHouseUserProfile>(new string[] { "point" });
        }
        #endregion

        #region user

        public const string SYSTEMUSERIDSTR="db9f8d40-94c2-4116-a5eb-897430f5624b";

        public Guid SystemUserId
        {
            get
            {
                return Guid.Parse(SYSTEMUSERIDSTR);
            }

        }


        /// <summary>
        /// just the create user method in the jtext103Identity, but with profile added
        //
        /// </summary>
        /// <param name="user">the oldhouse user</param>
        /// <param name="profileNames">you can indecate a list of new profile name</param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateUserWithProfile(OldHouseUser user, HashSet<string> profileNames)
        {
            //default user role
            user.Roles.Add("User");
            var avatarName = "defaultAvatar_boy_";
            if (user.sex == "female")
            {
                avatarName = "defaultAvatar_girl_";
            }
            var rnd = new Random();
            avatarName +=rnd.Next(1, 6).ToString()+".jpg";
            user.Avatar = avatarName;

            var result = await MyUserManager.CreateAsync(user);
            //only when the user is added then can we add profiels
            if (result.IsSuccessful)
            {
                //done add userId in profile
                user.Profiles = CreateNewProfile(profileNames,user.Id);
                await MyUserManager.UpdateAsync(user);
            }
            return result;
        }

        public OldHouseUser GetTestFackUser(string nickName, string userName, string password)
        {
            var user=MyUserManager.FindByNameAsync(userName).Result;
            if (user != null)
            {
                return user;
            }
            
            user =new OldHouseUser { NickName = nickName,
                UserName=userName,
                PasswordHash=password
                                };
            //todo put all oldhouse user roles in consts
            //todo use a fake user iu data base later
            user.Roles.Add("User");
            //it has no profile yet
            //todo may return a fake profile in data base
            var profile = CreateAFakeProfile();
            user.Profiles.Add(OldHouseUserProfile.PROFILENBAME, profile.Id);
            MyUserManager.CreateAsync(user).Wait();
            ProfileService.SaveOne(profile);
            return user;
        }

        public OldHouseUser GetTestFackUser()
        {
            return GetTestFackUser("Fake", @"fake@notrealemail.com", "password");
        }

        /// <summary>
        /// 授予/取消授予 用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        public void ToggoleUserRole(Guid userId, string role)
        {
            var user = MyUserManager.UserRepository.FindOneById(userId);
            if (user.Roles.Contains(role))
            {
                user.Roles.Remove(role);
            }
            else
            {
                user.Roles.Add(role);
            }
            MyUserManager.UserRepository.SaveOne(user);
        }

        /// <summary>
        /// 搜索用户（like用户名或昵称）
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<OldHouseUser> GetUserByNickNameOrUserName(string query, int pageIndex, int pageSize)
        {
            QueryObject<OldHouseUser> queryObject = new QueryObject<OldHouseUser>(MyUserManager.UserRepository);
            queryObject.AppendQuery(QueryOperator.Like, "NickName", query, QueryLogic.Or);
            queryObject.AppendQuery(QueryOperator.Like, "UserName", query, QueryLogic.Or);
            var result = MyUserManager.UserRepository.Find(queryObject, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// 获得搜索到用户的个数（like用户名或昵称）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public long GetUserCountByNickNameOrUserName(string query)
        {
            QueryObject<OldHouseUser> queryObject = new QueryObject<OldHouseUser>(MyUserManager.UserRepository);
            queryObject.AppendQuery(QueryOperator.Like, "NickName", query, QueryLogic.Or);
            queryObject.AppendQuery(QueryOperator.Like, "UserName", query, QueryLogic.Or);
            var result = MyUserManager.UserRepository.FindCountOfResult(queryObject);
            return result;
        }

        /// <summary>
        /// 通过得分对用户进行排序
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<OldHouseUser> GetAllUsersSortByPoint(int pageIndex, int pageSize)
        {
            var source = ProfileService.EntityRepository.FindAll("point", false, pageIndex, pageSize).ToArray();
            List<OldHouseUser> result = new List<OldHouseUser>();
            for (int i = 0; i < source.Count(); i++)
            {
                result.Add(MyUserManager.UserRepository.FindOneById(source[i].UserId));
            }
            return result;
        }

        /// <summary>
        /// 获得用户总数
        /// </summary>
        /// <returns></returns>
        public long GetAllUserCount()
        {
            long result = MyUserManager.UserRepository.FindAllCount();
            return result;
        }
        
        #endregion

        #region follow

        /// <summary>
        /// Am i following this user
        /// </summary>
        /// <param name="targetUserId">the followee user id</param>
        /// <param name="followerUserId">the follower userid</param>
        /// <returns></returns>
        public bool AmIFollowing(Guid targetUserId, Guid followerUserId)
        {
            var targetPid = MyUserManager.FindByIdAsync(targetUserId).Result.Profiles[OldHouseUserProfile.PROFILENBAME];
            var followerPid = MyUserManager.FindByIdAsync(followerUserId).Result.Profiles[OldHouseUserProfile.PROFILENBAME];
            return FollowService.AmIFollowing(targetPid,followerPid);
        }

        /// <summary>
        /// let the followerId follow the target id
        /// </summary>
        /// <param name="targetUserId">target(followee) user id</param>
        /// <param name="followerUserId">the follower user id</param>
        public void Follow(Guid targetUserId, Guid followerUserId)
        {
            //todo limit the following count here
            var targetPid = MyUserManager.FindByIdAsync(targetUserId).Result.Profiles[OldHouseUserProfile.PROFILENBAME];
            var followerPid = MyUserManager.FindByIdAsync(followerUserId).Result.Profiles[OldHouseUserProfile.PROFILENBAME];
            FollowService.Follow(targetPid,followerPid);
        }

        /// <summary>
        /// stop following the target
        /// </summary>
        /// <param name="targetUserId">target(followee) user id</param>
        /// <param name="followerUserId">the follower user id</param>
        public void UnFollow(Guid targetUserId, Guid followerUserId)
        {
            var targetPid = MyUserManager.FindByIdAsync(targetUserId).Result.Profiles[OldHouseUserProfile.PROFILENBAME];
            var followerPid = MyUserManager.FindByIdAsync(followerUserId).Result.Profiles[OldHouseUserProfile.PROFILENBAME];
            FollowService.UnFollow(targetPid, followerPid);
        }

        /// <summary>
        /// toggle the follow of a user
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <param name="followerUserId"></param>
        /// <returns></returns>
        public bool ToggoleFollow(Guid targetUserId, Guid followerUserId)
        {
            var targetPid = MyUserManager.FindByIdAsync(targetUserId).Result.Profiles[OldHouseUserProfile.PROFILENBAME];
            var user = MyUserManager.FindByIdAsync(targetUserId).Result;
            var followerPid = MyUserManager.FindByIdAsync(followerUserId).Result.Profiles[OldHouseUserProfile.PROFILENBAME];
            if (FollowService.AmIFollowing(targetPid, followerPid))
            {
                FollowService.UnFollow(targetPid, followerPid);
                return false;
            }
            else
            {
                FollowService.Follow(targetPid, followerPid);
                return true;
            }
        }

        /// <summary>
        /// get all followee ids for a follower
        /// </summary>
        /// <param name="followerUserId"></param>
        /// <returns></returns>
        public IEnumerable<Guid> GetAllFollowingIds(Guid followerUserId)
        {
            var followerPid = MyUserManager.FindByIdAsync(followerUserId).Result.Profiles[OldHouseUserProfile.PROFILENBAME];
            return FollowService.GetAllFollowingIds(followerPid);
        }

        /// <summary>
        /// 获得该用户主动follow（关注）的用户
        /// </summary>
        /// <param name="followerUserId"></param>
        /// <returns></returns>
        public IEnumerable<OldHouseUser> GetAllFollowingUser(Guid followerUserId)
        {
            //IEnumerable<Guid> followingIds = GetAllFollowingIds(followerUserId);
            //List<OldHouseUser> result = new List<OldHouseUser>();
            //foreach(Guid followingId in followingIds)
            //{
            //    result.Add(MyUserManager.FindByIdAsync(followingId).Result);
            //}
            //return result;

            IEnumerable<Guid> followingUserProfileIds = GetAllFollowingIds(followerUserId);
            List<OldHouseUser> result = new List<OldHouseUser>();
            foreach (Guid profileId in followingUserProfileIds)
            {
                Guid userId = ProfileService.FindOneById(profileId).UserId;
                result.Add(MyUserManager.FindByIdAsync(userId).Result);
            }
            return result;
        }

        /// <summary>
        /// 获得该用户主动follow（关注）的用户的数目
        /// </summary>
        /// <param name="followerUserId"></param>
        /// <returns></returns>
        public int GetAllFollowingUserCount(Guid followerUserId)
        {
            return GetAllFollowingIds(followerUserId).Count();
        }

        /// <summary>
        /// 获得关注该用户的所有用户id
        /// </summary>
        /// <param name="followeeUserId">该用户id</param>
        /// <returns></returns>
        public IEnumerable<Guid> GetAllFollowerUserIds(Guid followeeUserId)
        {
            //找到该用户的profile的id
            Guid followeeProfileId = MyUserManager.FindByIdAsync(followeeUserId).Result.Profiles[OldHouseUserProfile.PROFILENBAME];

            QueryObject<OldHouseUserProfile> queryObject = new QueryObject<OldHouseUserProfile>(ProfileService.EntityRepository);
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("FollowingIds", followeeProfileId);
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            List<Guid> userIds = new List<Guid>();
            var profiles = ProfileService.EntityRepository.Find(queryObject);
            foreach (var profile in profiles)
            {
                userIds.Add(profile.UserId);
            }
            return userIds;
        }


        #endregion
    }
}
