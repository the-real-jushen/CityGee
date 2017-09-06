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
        public Dictionary<string, Guid> CreateNewProfile(HashSet<string> profileNames)
        {
            var dict = new Dictionary<string, Guid>();
            foreach (var profileName in profileNames)
            {
                var profile = GetNewProfileWithoutSaveToDb(profileName);
                if (profile != null)
                {
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
                    return  new OldHouseUserProfile{ProfileName=profileName};
                default:
                    return null;
            }
        }

        public OldHouseUserProfile GetProfile(Guid profileId)
        {
            return ProfileService.FindOneById(profileId);
        }



        #endregion


        #region user

        /// <summary>
        /// just the create user method in the jtext103Identity, but with profile added
        //
        /// </summary>
        /// <param name="user">the oldhouse user</param>
        /// <param name="profileName">you can indecate a list of new profile name</param>
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
                user.Profiles = CreateNewProfile(profileNames);
                MyUserManager.UpdateAsync(user);
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
            MyUserManager.CreateAsync(user);
            ProfileService.SaveOne(profile);
            return user;
        }

        public OldHouseUser GetTestFackUser()
        {
            return GetTestFackUser("Fake", @"fake@notrealemail.com", "password");
        }



        #endregion
    }
}
