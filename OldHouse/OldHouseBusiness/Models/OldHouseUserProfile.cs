using Jtext103.EntityModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.Volunteer.Friend.Interfaces;
using Jtext103.Volunteer.VolunteerEvent;

namespace Jtext103.OldHouse.Business.Models
{
    /// <summary>
    /// the normal user for old house
    /// currently no service is needed just use a simple entity service to handle it,.
    /// later you can store avatar nickname that is already stored in user entity here, to override the one in user entity
    /// </summary>
    public class OldHouseUserProfile:Entity,IFollowable
    {

        public OldHouseUserProfile():base()
        {
            FollowingIds = new List<Guid>();
            point = 0.0;
        }

        public const string PROFILENBAME = "OldHouseUserProfile";
        //todo add application data here
        public string  ProfileName { get; set; }

        
        private double point;
        /// <summary>
        /// 该用户的得分
        /// </summary>
        public double Point
        {
            get
            {
                return point;
            }
        }

        /// <summary>
        /// 该用户等级
        /// </summary>
        public int Level
        {
            get
            {
                if (Point > 1000)
                {
                    return 5;
                }
                if (Point >= 500 && Point < 1000)
                {
                    return 4;
                }
                if (Point >= 200 && Point < 500)
                {
                    return 3;
                }
                if (Point >= 100 && Point < 200)
                {
                    return 2;
                }
                if (Point < 100)
                {
                    return 1;
                }
                return 1;
            }
        }

        /// <summary>
        /// to find the user easily
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 该用户主动follow（关注）的用户
        /// 存的是profile的id，而不是userId
        /// </summary>
        public IEnumerable<Guid> FollowingIds { get; set; }

        /// <summary>
        /// 该用户被follow（关注）的次数
        /// </summary>
        public int FollowerCount { get; set; }

        /// <summary>
        /// 为该用户加分，之后还需要save
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(double point)
        {
            this.point += point;
            //PointIncreaseEvent
            EventService.Publish("PointIncreaseEvent", this.Id, "system");
        }

        /// <summary>
        /// 用户被扣分，之后还需要save
        /// 最多扣到0分
        /// </summary>
        /// <param name="point"></param>
        public void MinusPoint(double point)
        {
            this.point -= point;
            if (this.point < 0)
            {
                this.point = 0.0;
            }
            //PointDecreaseEvent
            EventService.Publish("PointDecreaseEvent", this.Id, "system");
        }
    }
    
}