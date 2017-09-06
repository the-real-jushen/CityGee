using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    /// <summary>
    /// this is not only used in rating thing, it also used in Like and Favorite
    /// </summary>
    public class LrfDto
    {
        /// <summary>
        /// the target id you what to rate or like or favorite
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// the rating in float, only used if you are rating things, or display ratings
        /// </summary>
        public float Rating{ get; set; }

        /// <summary>
        /// how many users have Lrf this entity
        /// </summary>
        public int LrfCount { get; set; }

        /// <summary>
        /// did i rate live fav or read this entity?
        /// </summary>
        public bool DidILrf { get; set; }

        /// <summary>
        /// is it a House, CheckIn in or anything?
        /// </summary>
        [Obsolete]
        public string TargetType { get; set; }
    }
}