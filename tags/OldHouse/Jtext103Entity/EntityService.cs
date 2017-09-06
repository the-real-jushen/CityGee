using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.Repository.Interface;
using Jtext103.Repository;

namespace Jtext103.EntityModel
{
    /// <summary>
    /// services that uses only one reposorty can inherit this class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityService<T> where T:Entity
    {
        public IRepository<T> EntityRepository;

        public EntityService(IRepository<T> repository)
        {
            EntityRepository = repository;
            
        }

        public virtual void RegisterField<T2>(List<string> fields) where T2 : T
        {
            if (fields.Any())
            {
                EntityRepository.RegisterMap<T2>(fields);
            }
            else
            {
                EntityRepository.RegisterMap<T2>();
            }
            
        }



        #region insert&save&update
        public virtual void InsertOne(T entity)
        {
            EntityRepository.InsertOne(entity);
        }

        public virtual void SaveOne(T entity)
        {
            EntityRepository.SaveOne(entity);
        }

        public virtual void UpdateOne(Guid id, Dictionary<string, object> updateObject)
        {
            Dictionary<string, object> queryObject = new Dictionary<string, object>();
            queryObject.Add("_id", id);
            EntityRepository.Update(queryObject, updateObject);
        }
        #endregion insert&save&update

        #region find
        public virtual T FindOneById(Guid id)
        {
            return EntityRepository.FindOneById(id);
        }

        public virtual long FindAllCount()
        {
            return EntityRepository.FindAllCount();
        }

        #endregion
        
        #region delete
        public virtual void Delete(Guid id)
        {
            QueryObject<T> query = new QueryObject<T>(EntityRepository);
            var qdict=new Dictionary<string,object>();
            qdict.Add("_id",id);
            query.AppendQuery(qdict,QueryLogic.And);
            EntityRepository.Delete(query);
        }

        public virtual void Delete(T entity)
        {

            EntityRepository.Delete(entity);
        }

        #endregion

        #region helper
        protected QueryObject<T> getNewQueryObject()
        {
            QueryObject<T> query = new QueryObject<T>(EntityRepository);
            return query;

        }

        #endregion

    }
}
