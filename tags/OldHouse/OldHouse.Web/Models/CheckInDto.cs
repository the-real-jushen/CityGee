using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OldHouse.Web.Models
{
    /// <summary>
    /// used to create, list, detail a check in
    /// </summary>
    public class CheckInDto
    {
        //todo model anotation is need here
        public Guid TargetId { get; set; }
        public Guid UserId { get; set; }
        
        public Guid Id { get; set; }
        public string Titile { get; set; }

        
        [Required]
        [StringLength(1000, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [Display(Name = "说点什么吧～")]
        public string   Content { get; set; }

        public string   Lnt { get; set; }
        public string Lat { get; set; }
        /// <summary>
        /// currently this is only used for display
        /// </summary>
        public string Distance { get; set; }

        public string[] Images { get; set; }

        /// <summary>
        /// this is purely for seo, to display house name in title
        /// </summary>
        public string HouseName { get; set; }
    }
}