using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace OldHouse.Web.Models
{
    public class PageControl
    {
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
        public int PageSize { get; set; }

        public string RouteName{get;set;}

        public Dictionary<string,object> RouteValue { get; set; }
        public bool UseAjax { get; set; }
        public string PageContentId { get; set; }
        public PageControl(int currentPage, int lastPage, int pageSize)
        {
            CurrentPage = currentPage;
            LastPage = lastPage;
            PageSize = pageSize;
            PageContentId = "pageContent";
        }
    }
}