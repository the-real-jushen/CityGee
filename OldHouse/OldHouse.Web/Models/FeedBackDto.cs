using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    public class FeedBackDto
    {
        public Guid Id { get; set; }
        public string CreatedUrl { get; set; }
        public string Content { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid UserId { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
    }
}