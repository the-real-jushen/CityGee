using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using Jtext103.OldHouse.Business.Services;
using OldHouse.Web.Models;
using Microsoft.Owin.Host.SystemWeb;
using OldHouse.Web.App_Start;


namespace OldHouse.Web.Controllers
{
    public class LocationController : ApiController
    {
        /// <summary>
        /// get the cached location, the location is based on IP address, so if your ip address is not changed bu you traveled a long way you should update the location
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public LocationDto CachedLocation()
        {
            var key =HttpContext.Current.GetOwinContext().Request.RemoteIpAddress + "_Location";
            return HttpContext.Current.Cache[key] as LocationDto;
        }


        /// <summary>
        /// update the location cache for the requested ip
        /// </summary>
        /// <param name="location">the new location</param>
        [HttpPost]
        [ActionName("CachedLocation")]
        public void UpCachedLocation(LocationDto location)
        {
            if (HttpContext.Current.Cache[getCacheLocationKey()] == null)
            {
                //todo revise the hardcode
                HttpContext.Current.Cache.Add(getCacheLocationKey(), location, null, Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(10), CacheItemPriority.Low, null);
            }
            else
            {
                HttpContext.Current.Cache[getCacheLocationKey()] = location;
            }
        }



        /// <summary>
        /// return the distance of a 2 points, in km
        /// </summary>
        /// <param name="loc1">"lng;lat"</param>
        /// <param name="loc2">"lng;lat"</param>
        /// <returns>distance in km</returns>
        [HttpGet]
        public string DistanceBetween(string loc1,string loc2)
        {
            var point1 = HouseService.GetGeoPoint(loc1);
            var point2 = HouseService.GetGeoPoint(loc2);
            return HouseService.GetDistanceKm(point1, point2);
        }


        #region helper

        private string getCacheLocationKey()
        {
            return HttpContext.Current.GetOwinContext().Request.RemoteIpAddress + "_Location";
        }

        #endregion
    }
}
