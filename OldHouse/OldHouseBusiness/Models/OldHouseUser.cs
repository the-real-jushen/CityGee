using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.EntityModel;
using Jtext103.Identity.Interfaces;
using Jtext103.Identity.Models;

namespace Jtext103.OldHouse.Business.Models
{
    public  class OldHouseUser:Entity,IUser
    {

        public OldHouseUser()
        {
            Roles=new HashSet<string>();
            Stamp = new SecurityStamp();
            Roles = new HashSet<string>();
            Profiles = new Dictionary<string, Guid>();
            Avatar = "DefaultAvatar.jpg";
            sex = "female";
        }
        public string PasswordHash
        {
            get;
            set;
        }

        public HashSet<string> Roles
        {
            get;
            set;
        }

        public SecurityStamp Stamp
        {
            get;
            set;
        }

        /// <summary>
        /// this for oldhoues is the user email and must be unique
        /// </summary>
        public string UserName
        {
            get;
            set;
        }


        #region OldHouse Application specific

        /// <summary>
        /// a displayed name for user can be dupplicated.
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// the avatar file name
        /// </summary>
        public string   Avatar { get; set; }          
        /// <summary>
        /// in oldhouse profile is entity, 
        /// the key is profile anem, the alue is the profile ID
        /// </summary>
        public Dictionary<string,Guid> Profiles { get; set; }

        public string sex { get; set; }


        #endregion

    }
}
