using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Jtext103.Repository.Interface;
using Jtext103.Repository;
using Jtext103.OldHouse.Business.Models;
using Jtext103.EntityModel;
using Jtext103.BlogSystem;

namespace Jtext103.OldHouse.Business.Services
{
    /// <summary>
    /// the code about checkin for house service 
    /// !!!Mainly works with the _checkInService which is a blog servie, this can be refactored into a new service if it gets to fat
    /// </summary>
    public partial class HouseService
    {
        #region checkin

        public void CheckInHouse(Guid housId, CheckIn checkIn)
        {
            //the house id can be used to modify some properties in the house entity
            checkIn.TargetId = housId;
            CheckInService.SaveOne(checkIn);
            
        }

        public void CheckInHouse(Guid housId, Guid userId, string content,List<Asset> asset,GeoPoint  location)
        {
            //map the application user to this basic user
            var user= new BasicUser(userId, "!!Some User Name", "!!Some Nick name");
            var checkin = new CheckIn(user, housId, "some checkin!!", content, asset, location);
            CheckInHouse(housId, checkin);
        }

        public IEnumerable<BlogPostEntity> ListCheckInsFor(Guid houseId,int page,int pagesize)
        {
            //sorting
            return CheckInService.FindAllBlogPostFor(houseId, "", false, page, pagesize);
        }

        public int GetCheckInCountFor(Guid houseId)
        {
            return (int)CheckInService.FindAllBlogPostCountFor(houseId);
        }

        #endregion
    }
}
