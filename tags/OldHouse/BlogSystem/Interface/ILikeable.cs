﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.BlogSystem.Interface
{
    public interface ILikeable
    {
        //点赞的次数
        int LikedTimes { get; set; }
        
    }
}
