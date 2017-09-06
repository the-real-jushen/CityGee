using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    /// <summary>
    /// hold th info of the latest app
    /// ,naming convension is java compatible
    /// </summary>
    public class UpgradeInfo
    {
        public string versionName;
        public int versionNum;
        public string releaseNote;
    }

}