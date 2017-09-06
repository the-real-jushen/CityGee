using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OldHouse.Web.Models
{
    public class UserProfileDto
    {
        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string UserName { get; set; }
        [Display(Name = "昵称")]
        [Required]
        public string NickName { get; set; }
        [Display(Name = "性别")]
        [Required]
        public string Sex { get; set; }
        [Display(Name = "省份")]
        [Required]
        public string ProfileProvince { get; set; }
        [Display(Name = "城市")]
        [Required]
        public string ProfileCity { get; set; }
    }
}