using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using Jtext103.OldHouse.Business.Services;
using OldHouse.Web.Models;
using Microsoft.Owin.Host.SystemWeb;
using Newtonsoft.Json;
using OldHouse.Web.App_Start;
using OldHouse.Web.Controllers.API;
using Jtext103.OldHouse.Business.Models;
using Microsoft.Owin;

namespace OldHouse.Web.Controllers
{
    public class LocationController : BaseApiController
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



        private LocationDto callIpLocationApi(string ip)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(15);
            ip = (ip != "::1") ? ip : "115.156.252.5";
            var url = @"https://freegeoip.net/json/" + ip;

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
            };
            request.Headers.AcceptLanguage.TryParseAdd("zh-CN");
            HttpResponseMessage response;
            try
            {

                response = httpClient.SendAsync(request).Result;
            }
            catch
            {
                throw new Exception("ip location API error");
            }
            string reponseStringSource = response.Content.ReadAsStringAsync().Result;
            dynamic loc = JsonConvert.DeserializeObject(reponseStringSource);
            var ipLoc = new LocationDto { Latitude = loc.latitude, Longitude = loc.longitude, Country = loc.country_name, Province = loc.region_name, City = loc.city };
            return ipLoc;
        }



        /// <summary>
        /// if the user browser cannot get a geolocation, it will try to get, the ip loction
        /// it will be cached automatically
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetAndCacheIpLocation()
        {
            LocationDto ipLoc;
            try
            {
                ipLoc = callIpLocationApi(HttpContext.Current.GetOwinContext().Request.RemoteIpAddress);
            }
            catch (Exception)
            {
                return InternalServerError();
            } 
            UpCachedLocation(ipLoc);
            ipLoc.IpAddress = HttpContext.Current.GetOwinContext().Request.RemoteIpAddress;
            return Ok(ipLoc);
        }



        /// <summary>
        /// get the ip location, but this will not cache the coorinates and only returns the location, 
        /// but it will update the region info if there is aready a cached location for this ip
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetIpLocation()
        {
            //var cached = CachedLocation();
            //if (cached != null && string.IsNullOrEmpty(cached.City)==false )
            //{
            //    return Ok(cached);
            //}
            LocationDto ipLoc;
            try
            {
                ipLoc = callIpLocationApi(HttpContext.Current.GetOwinContext().Request.RemoteIpAddress);
            }
            catch (Exception)
            {
                return InternalServerError();
            } 
            tryUpdatecachedLocationRegionInfo(ipLoc);
            ipLoc.IpAddress = HttpContext.Current.GetOwinContext().Request.RemoteIpAddress;
            return Ok(ipLoc);
        }



        private void tryUpdatecachedLocationRegionInfo(LocationDto region)
        {
            var cached = CachedLocation();
            if (cached != null)
            {
                cached.Country = region.Country;
                cached.City = region.City;
                cached.Province = region.Province;
                UpCachedLocation(cached);
            }
        }

        /// <summary>
        /// RefreshUserCity
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public void RefreshUserCity(CityModel model)
        {
            if(AppUser != null)
            {
                MyService.AddOrModifyCurrentCityForProfile(AppUser.Profiles[OldHouseUserProfile.PROFILENBAME], model.currentCity);
                MyService.AddOrModifyCurrentProvinceForProfile(AppUser.Profiles[OldHouseUserProfile.PROFILENBAME], model.currentProvince);
            }
            //var cookie = new HttpCookie("citygee-currentprovince");
            //cookie.Value = model.currentProvince;
            //cookie.Expires = DateTime.Now.AddDays(7);
            //HttpContext.Current.Response.Cookies.Add(cookie);
            //cookie = new HttpCookie("citygee-currentcity");
            //cookie.Value = model.currentCity;
            //cookie.Expires = DateTime.Now.AddDays(7);
            //HttpContext.Current.Response.Cookies.Add(cookie);

            //HttpCookie cookie = new HttpCookie("citygee-currentcity", model.currentCity);
            //cookie.Expires = DateTime.Now.AddDays(1);
            //HttpContext.Current.Response.AppendCookie(cookie);
        }

        //public HttpResponseMessage Get()
        //{
        //    var resp = new HttpResponseMessage();

        //    var cookie = new CookieHeaderValue("session-id", "12345");
        //    cookie.Expires = DateTimeOffset.Now.AddDays(1);
        //    cookie.Domain = Request.RequestUri.Host;
        //    cookie.Path = "/";

        //    resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
        //    return resp;
        //}


        #region helper


        private string getCacheLocationKey()
        {
            return HttpContext.Current.GetOwinContext().Request.RemoteIpAddress + "_Location";
        }

        #endregion
    }

    public class CityModel
    {
        public string currentProvince { get; set; }
        public string currentCity { get; set; }
    }
}
