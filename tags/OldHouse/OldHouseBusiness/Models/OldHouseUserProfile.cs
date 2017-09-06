using Jtext103.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.OldHouse.Business.Models
{
    /// <summary>
    /// the normal user for old house
    /// currently no service is needed just use a simple entity service to handle it,.
    /// later you can store avatar nickname that is already stored in user entity here, to override the one in user entity
    /// </summary>
    public class OldHouseUserProfile:Entity
    {

        public const string PROFILENBAME = "OldHouseUserProfile";
        //todo add application data here
        public string  ProfileName { get; set; }

    }
    
}