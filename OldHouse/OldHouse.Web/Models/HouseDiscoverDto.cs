using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OldHouse.Web.Models
{
    /// <summary>
    /// used to 
    /// </summary>
    public class HouseDiscoverDto
    {
        /// <summary>
        /// House guid
        /// </summary>
        [Required]
        public string Id { get; set; }
         /// <summary>
        /// the user guid whom added by
        /// </summary>
        [Required]
        public string OwnerId { get; set; }
        /// <summary>
        /// Is aproved by editor
        /// </summary>
        [Required]
        public string IsApproved { get; set; }

        /// <summary>
        /// need register
        /// </summary>
        //private double _overallValue;
        [Required]
        [Display(Name = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// a string describe this location
        /// </summary>
        [Required]
        public string LocationString { get; set; }
        /// <summary>
        /// the bloe 3 location related prop should be auto-filled
        /// </summary>
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }

        [Required]
        [Display(Name = "简介")]
        public string Abstarct { get; set; }

        [AllowHtml]
        [Display(Name = "描述")]
        public string Description { get; set; }

        //public string Condition { get; set; }
        [Required]
        [Display(Name = "建造年份")]
        public int BuiltYear { get; set; }


        //public double HistoricValue{ get; set; }
        //public double PhotoValue { get; set; }

        public List<string> Images { get; set; }
        public string Tags { get; set; }
        
        /// <summary>
        /// one of the images user uploaded,
        /// better if can be choosed
        /// </summary>
        [Required]
        public string Cover { get; set; }
        [Required]
        public float Rating { get; set; }
        /// <summary>
        /// a unique code name for a house, unlike the ID its's human readable
        /// for user discoveed houses, this will be generated using a slug generater
        /// /// </summary>
        public string CodeName { get; set; }

        /// <summary>
        /// longitude
        /// </summary>
        [Required]
        public string Lnt { get; set; }
        
        /// <summary>
        /// latitude
        /// </summary>
        [Required]
        public string Lat { get; set; }
    }
}