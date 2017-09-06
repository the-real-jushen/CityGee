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
    public class CommentService:BlogBaseService<CommentEntity>
    {
        public CommentService(IRepository<CommentEntity> repository,LikeRateService likeService )
            : base(repository,likeService)
        {

        }

        public virtual void AddComment(BasicUser user, Guid targetId, Guid fatherId, string content)
        {
            //先计算楼层
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("TargetId", targetId);
            QueryObject<CommentEntity> queryObject = new QueryObject<CommentEntity>(EntityRepository);
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            int position = Convert.ToInt32(EntityRepository.FindCountOfResult(queryObject)) + 1;
            //加入数据库
            CommentEntity newComment = new CommentEntity(user, targetId, fatherId, content);
            newComment.Position = position;
            EntityRepository.SaveOne(newComment);
            //当存在father时
            //在father的children中加入该comment
            if (fatherId != Guid.Empty)
            {
                CommentEntity father = FindOneById(fatherId);
                father.ChildrenId.Add(newComment.Id);
                EntityRepository.SaveOne(father);
            }
        }


        /// <summary>
        /// 将comment标记为已删除
        /// </summary>
        public void DeleteComment(Guid commentId)
        {
            CommentEntity commentEntity = FindOneById(commentId);
            commentEntity.HasDeleted = true;
            EntityRepository.SaveOne(commentEntity);
        }


        /// <summary>
        /// 生成该comment所显示的内容
        /// </summary>
        public virtual string GenerateDisplayContent(CommentEntity commentEntity)
        {
            if (commentEntity.HasDeleted == true)
            {
                return "该评论已被删除！";
            }
            //如果没有father，直接返回该comment的content
            if (commentEntity.FatherId == Guid.Empty)
            {
                return commentEntity.Content;
            }
            //如果有father，需要在content中显示father的名字
            else
            {
                string fatherName = FindOneById(commentEntity.FatherId).User.NickName;
                string content = "【回复 " + fatherName + "】" + commentEntity.Content;
                return content;
            }
        }


        #region find comment
        public virtual CommentEntity FindComment(Guid commentId)
        {
            CommentEntity result = EntityRepository.FindOneById(commentId);
            return result;
        }

        /// <summary>
        /// 所有对targetId的评论的个数
        /// </summary>
        public long FindAllCommentsCount(Guid targetId)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("TargetId", targetId);
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            long result = EntityRepository.FindCountOfResult(queryObject);
            return result;
        }

        /// <summary>
        /// 找到所有对targetId的评论
        /// </summary>
        public IEnumerable<CommentEntity> FindAllComments(Guid targetId, string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("TargetId", targetId);
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            IEnumerable<CommentEntity> result = EntityRepository.Find(queryObject, sortByKey, isAscending, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// 找到所有对targetId的根评论
        /// </summary>
        public IEnumerable<CommentEntity> FindAllRootComments(Guid targetId, string sortByKey, bool isAscending, int pageIndex, int pageSize)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("TargetId", targetId);
            queryDict.Add("FatherId", Guid.Empty);
            var queryObject = getNewQueryObject();
            queryObject.AppendQuery(queryDict, QueryLogic.And);
            IEnumerable<CommentEntity> result = EntityRepository.Find(queryObject, sortByKey, isAscending, pageIndex, pageSize);
            return result;
        }
        #endregion find comment




    }
}
