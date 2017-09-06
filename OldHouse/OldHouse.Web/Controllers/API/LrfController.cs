using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Jtext103.BlogSystem;
using Jtext103.BlogSystem.Extension;
using OldHouse.Web.Models;

namespace OldHouse.Web.Controllers.API
{
    /// <summary>
    /// to like, rate, favorite or read an entity
    /// </summary>
    public class LrfController : BaseApiController
    {
        /// <summary>
        /// rate a house
        /// lrf/house/rate
        /// </summary>
        /// <param name="rateDto">the target id and the type, you can only rate a house now, this rating relies on entity service ont only the lrf service</param>
        /// <returns>the new rating</returns>
        [Authorize]
        [HttpPost]
        [ActionName("Rate")]
        public float RateHouse(LrfDto rateDto)
        {
            return MyService.Rate(rateDto.Id, AppUser.Id, MyService.LrfService, rateDto.Rating);
        }

        /// <summary>
        /// get the current user rating for a house,!! worksonly on house, try not use this, you get rating from the house entity
        /// lrf/my/house/rate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Rate")]
        [Authorize]
        public float GetHouseRate(Guid id)
        {
            return MyService.LrfService.GetMyRatingFor(AppUser.Id, id);
        }

        
        /// <summary>
        /// toggle the like of the entity
        /// lrf/toggelLiek
        /// </summary>
        /// <param name="rateDto"></param>
        /// <returns>the new like or not, and times</returns>
        [Authorize]
        [HttpPost]
        public LrfDto ToggleLike(LrfDto rateDto)
        {
             
            var didILrf = MyService.LrfService.ToggleLike(AppUser.Id, rateDto.Id);
            var lrfTimes = MyService.LrfService.GetLRFCount(rateDto.Id, LRFType.Like);
            return new LrfDto { DidILrf = didILrf, LrfCount = lrfTimes };
        }


        /// <summary>
        /// toggle the favorate of an entity
        /// lef/toggleFavorite
        /// </summary>
        /// <param name="rateDto"></param>
        /// <returns>the new fav or not and times</returns>
        [Authorize]
        [HttpPost]
        public LrfDto ToggleFavorite(LrfDto rateDto)
        {
            var didILrf= MyService.LrfService.ToggleFavorite(AppUser.Id, rateDto.Id);
            var lrfTimes = MyService.LrfService.GetLRFCount(rateDto.Id, LRFType.Favorate);
            return new LrfDto {DidILrf = didILrf, LrfCount = lrfTimes};
        }

        /// <summary>
        /// read a entity only works for logged in user
        /// lrf/read
        /// </summary>
        /// <param name="rateDto"></param>
        /// <returns>the new fav or not</returns>
        [Authorize]
        [HttpPost]
        public void Read(LrfDto rateDto)
        {
            MyService.LrfService.Read(AppUser.Id, rateDto.Id);
        }

        /// <summary>
        /// get the Like,Rate, Favorite, Read times for an entity
        /// lrf/times/{type}/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">Like,Rate, Favorite, Read</param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Times")]
        public int GetLrfTime(Guid id, string type)
        {
            switch (type)
            {
                case "Rate":
                    return MyService.LrfService.GetLRFCount(id, LRFType.Rate);
                case "Like":
                    return MyService.LrfService.GetLRFCount(id, LRFType.Like);
                case "Favorite":
                    return MyService.LrfService.GetLRFCount(id, LRFType.Favorate);
                case "Read":
                    return MyService.LrfService.GetLRFCount(id, LRFType.Read);
                default:
                    return -1;
            }
        }

        /// <summary>
        /// get if the current user have Like,Rate, Favorite, Read this entity
        /// lrf/my/{type}/{id}
        /// </summary>
        /// <param name="id">entity id</param>
        /// <param name="type">Like,Rate, Favorite, Read</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public bool Get(Guid id, string type)
        {
            switch (type)
            {
                case "Rate":
                    return MyService.LrfService.DoILikeRateFav(AppUser.Id, id, LRFType.Rate);
                case "Like":
                    return MyService.LrfService.DoILikeRateFav(AppUser.Id, id, LRFType.Like);
                case "Favorite":
                    return MyService.LrfService.DoILikeRateFav(AppUser.Id, id, LRFType.Favorate);
                case "Read":
                    return MyService.LrfService.DoILikeRateFav(AppUser.Id, id, LRFType.Read);
                default:
                    return false;
            }
        }

        



    }
}
