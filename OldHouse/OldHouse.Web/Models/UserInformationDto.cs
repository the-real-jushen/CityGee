using Jtext103.BlogSystem;
using Jtext103.OldHouse.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    public class UserInformationDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }

        public string Sex { get; set; }
        public string Who { get; set; }
        public int BeFollowedCount { get; set; }
        public int BeLikedCount { get; set; }
        public double MyPoint { get; set; }
        public int MyRank { get; set; }
        public int MyLevel { get; set; }
        public int FollowCount { get; set; }
        public int GrantedDiscoveryCount { get; set; }
        public int DiscoveryCount { get; set; }
        public int GrantedCheckinCount { get; set; }
        public int CheckinCount { get; set; }
        public int LikedHouseCount { get; set; }
        public int LikedCheckinCount { get; set; }

        public bool IsDeveloper { get; set; }
        public bool IsEditor { get; set; }
        public bool IsAdmin { get; set; }
    }
}