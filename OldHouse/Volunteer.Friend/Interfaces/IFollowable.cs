using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.Volunteer.Friend.Interfaces
{
    /// <summary>
    /// if the user or the user profile want to support following, you have to implement this interface
    /// </summary>
    public interface IFollowable
    {
        IEnumerable<Guid> FollowingIds { get; set; }
        int FollowerCount { get; set; }
    }
}
