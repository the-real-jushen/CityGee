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
        //done model anotation is need here
        public Guid TargetId { get; set; }//houseId
        public Guid UserId { get; set; }
        public string UserAvatar { get; set; }
        public string UserNickName { get; set; }

        public Guid Id { get; set; }
        public string Titile { get; set; }
        public string ModifyTime { get; set; }
        
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

        public List<string> Images { get; set; }

        /// <summary>
        /// this is information for house
        /// </summary>
        public string HouseName { get; set; }
        public string LocationString { get; set; }
        public string HouseCover { get; set; }
        public bool IsEssence { get; set; }
    }
}