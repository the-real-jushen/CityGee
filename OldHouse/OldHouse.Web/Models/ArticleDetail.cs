using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    public class ArticleDetail
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorAvatar { get; set; }
        public string AuthorNickNmae { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}