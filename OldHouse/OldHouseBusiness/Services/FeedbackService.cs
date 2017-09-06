using Jtext103.EntityModel;
using Jtext103.OldHouse.Business.Models;
using Jtext103.Repository;
using Jtext103.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.OldHouse.Business.Services
{
    public class FeedbackService : EntityService<FeedBackEntity>
    {
        private static FeedbackService instence;
        public static FeedbackService Instence
        {
            get { return instence; }
        }

        public FeedbackService(IRepository<FeedBackEntity> repository): base(repository)
        {
            instence = this;
        }

        /// <summary>
        /// 找到所有的FeedBack的个数
        /// </summary>
        public long FindAllFeedBackCountFor()
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            long result = EntityRepository.FindCountOfResult(queryObject);
            return result;
        }

        /// <summary>
        /// 找到所有的FeedBack
        /// </summary>
        public virtual IEnumerable<FeedBackEntity> FindAllFeedBackFor(string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            IEnumerable<FeedBackEntity> result = EntityRepository.Find(queryObject, sortByKey, isAscending, pageIndex, pageSize);
            return result;
        }
    }
}
