using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    public class UserDisplayDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string Sex { get; set; }
        public string Who { get; set; }
        public int DiscoveryCount { get; set; }
        public int CheckinCount { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsEditor { get; set; }
        public bool IsAdmin { get; set; }
        public string ProfileProvince { get; set; }
        public string ProfileCity { get; set; }
    }
}