using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OldHouse.Web.Models
{
    public class ReleaseFeedDto
    {
        [Required]
        [Display(Name = "是否广播")]
        public bool IsBroadcast { get; set; }

        [Required]
        [Display(Name = "标题")]
        public string FeedTitle { get; set; }

        [Required]
        [Display(Name = "正文")]
        public string FeedText { get; set; }

        [Display(Name = "图片")]
        public List<string> Images { get; set; }

        [Required]
        [Display(Name = "链接")]
        public string DestinationLink { get; set; }

        [Required]
        [Display(Name = "是否新窗口打开")]
        public bool NewBlank { get; set; }

        [Display(Name = "接受者邮箱")]
        [DataType(DataType.EmailAddress)]
        public string ReceiverEmail { get; set; }

        public ReleaseFeedDto()
        {
            IsBroadcast = false;
            NewBlank = false;
            DestinationLink = "#";//链接为空
        }
    }
}