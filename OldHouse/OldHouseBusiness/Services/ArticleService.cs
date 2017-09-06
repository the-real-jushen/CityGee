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
    public class ArticleService : EntityService<Article>
    {
        private static ArticleService instence;
        public static ArticleService Instence
        {
            get { return instence; }
        }

        public ArticleService(IRepository<Article> repository)
            : base(repository)
        {
            instence = this;
        }

        /// <summary>
        /// 找到所有的Article的个数
        /// </summary>
        public long FindAllArticleCount()
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            long result = EntityRepository.FindCountOfResult(queryObject);
            return result;
        }

        /// <summary>
        /// 找到所有的Article
        /// </summary>
        public virtual IEnumerable<Article> FindAllArticle(string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            IEnumerable<Article> result = EntityRepository.Find(queryObject, sortByKey, isAscending, pageIndex, pageSize);
            return result;
        }

    }
}
