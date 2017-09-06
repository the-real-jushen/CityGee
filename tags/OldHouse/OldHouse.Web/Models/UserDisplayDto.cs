using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace OldHouse.Web.Models
{
    /// <summary>
    /// this is used to display user infor 
    /// later profile can be also mapped to this object
    /// 
    /// </summary>
    public class UserDisplayDto
    {
        public string UserName { get; set; }
        public Guid Id { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
    }
}