using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.EntityModel;
using Jtext103.Repository.Interface;
using Jtext103.Repository;

namespace Jtext103.BlogSystem
{
    /// <summary>
    /// use this to handle favorate like ratings, user can rate multiple times but it is very un-recommanded
    /// </summary>
    public class LikeRateService:EntityService<LikeRateEntity>
    {
        public LikeRateService(IRepository<LikeRateEntity> repository):base(repository)
        {
            
        }

        /// <summary>
        /// return if i rated a target
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public virtual bool DoIRated(Guid userId, Guid targetId)
        {
            
            Dictionary<string,object> queryDict=new Dictionary<string,object>();
            queryDict.Add("Ratings.RatedBy",userId);
            queryDict.Add("Id",targetId);
            var query=getNewQueryObject();
            query.AppendQuery(queryDict,QueryLogic.And);
            return EntityRepository.Find(query).Any();
        }

        /// <summary>
        /// get my rating for a target, will throw if you have not rated,if you rated more than onec this will return the first one
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public virtual double GetMyRatingFor(Guid userId, Guid targetId)
        {

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", targetId);
            var query = getNewQueryObject();
            query.AppendQuery(queryDict, QueryLogic.And);
            var rate =EntityRepository.Find(query).FirstOrDefault();
            var rating =rate.Ratings.Where(r=>r.RatedBy==userId).FirstOrDefault();
            if (rating != null)
            {
                return rating.MyRating;
            }
            else
            {
                throw new System.Exception("Not Found");
            }
        }

        public virtual int GetRatedCount(Guid targetId)
        {
            return FindOneById(targetId).Ratings.Count();
        }

        public virtual List<Rating> GetAllRatingFor(Guid targetId)
        {
            return FindOneById(targetId).Ratings;
        }

        
        /// <summary>
        /// clear all my ratings, return the count of ratings that is removed
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <returns>the count of the ratings cleared</returns>
        public virtual int ClearMyRate(Guid userId, Guid targetId)
        {
            int clearCount = 0;
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", targetId);
            var query = getNewQueryObject();
            query.AppendQuery(queryDict, QueryLogic.And);
            var rate = EntityRepository.Find(query).FirstOrDefault();
            if (rate == null)
            {
                List<Rating> indexes=new List<Rating> ();
                for (int i=0;i<rate.Ratings.Count;i++)
                {
                    if (rate.Ratings[i].RatedBy == userId)
                    {
                        indexes.Add(rate.Ratings[i]);
                        clearCount++;
                    }
                }
                foreach (var item in indexes)
                {
                    rate.Ratings.Remove(item);

                }
                EntityRepository.SaveOne(rate);
                
            }
            return clearCount;
        }

        /// <summary>
        /// rate a stuff
        /// </summary>
        /// <param name="userId">user rated it</param>
        /// <param name="targetId">the stuff </param>
        /// <param name="rating"></param>
        /// <param name="allowedMultiRate"></param>
        /// <returns></returns>
        public bool Rate(Guid userId, Guid targetId, double rating,bool allowedMultiRate = false)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Id", targetId);
            var query = getNewQueryObject();
            query.AppendQuery(queryDict, QueryLogic.And);
            var rate = EntityRepository.Find(query).FirstOrDefault();
            if (rate == null)
            {
                //create new target and save
                List<Rating> newRate=new List<Rating>();
                newRate.Add(new Rating{RatedBy=userId,MyRating=rating});
                this.InsertOne(new LikeRateEntity {Id=targetId,Ratings=newRate });
                return true;
            }
            else
            {
                if (allowedMultiRate == true || DoIRated(userId,targetId)==false)
                {
                    rate.Ratings.Add(new Rating { RatedBy = userId, MyRating = rating });
                    this.SaveOne(rate);
                    return true;
                }
            }
            return false;
        }



    }
}
