using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OldHouse.Web.Models
{
    public class ReleaseArticleDto
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "标题")]
        public string ArticleTitle { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "正文")]
        public string ArticleBody { get; set; }

        [Required]
        [Display(Name = "是否发送广播Feed")]
        public bool IsReleaseBroadcastFeed { get; set; }

        public ReleaseArticleDto()
        {
            IsReleaseBroadcastFeed = true;
        }
    }
}