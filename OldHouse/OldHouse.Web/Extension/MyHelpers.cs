using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NPinyin;
using OldHouse.Web.Models;

namespace OldHouse.Web.Extension
{
    public static class UrlHelperExtensions  
    {
        //Builds URL by finding the best matching route that corresponds to the current URL,
        //with given parameters added or replaced.
        public static MvcHtmlString Current(this UrlHelper helper, object substitutes)
        {
            //get the route data for the current URL e.g. /Research/InvestmentModelling/RiskComparison
            //this is needed because unlike UrlHelper.Action, UrlHelper.RouteUrl sets includeImplicitMvcValues to false
            //which causes it to ignore current ViewContext.RouteData.Values
            var rd = new RouteValueDictionary(helper.RequestContext.RouteData.Values);

            //get the current query string e.g. ?BucketID=17371&amp;compareTo=123
            var qs = helper.RequestContext.HttpContext.Request.QueryString;

            //add query string parameters to the route value dictionary
            foreach (string param in qs)
                if(!string.IsNullOrEmpty(qs[param]))
                    rd[param] = qs[param];

            //override parameters we're changing
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
            {
                var value = property.GetValue(substitutes) == null ? null : property.GetValue(substitutes).ToString();
                if (string.IsNullOrEmpty(value))
                    rd.Remove(property.Name);
                else
                    rd[property.Name] = value;
            }

            //UrlHelper will find the first matching route
            //(the routes are searched in the order they were registered).
            //The unmatched parameters will be added as query string.
            //rd.Add("area","House");
            var url = helper.RouteUrl(rd);
            return new MvcHtmlString(url);
        }


        public static MvcHtmlString GetPagingRoute(this UrlHelper helper, object substitutes,PageControl pageControl)
        {
            if (string.IsNullOrEmpty(pageControl.RouteName))
            {
                return helper.Current(substitutes);
            }
            else
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
                {
                    var value = property.GetValue(substitutes) == null ? null : property.GetValue(substitutes).ToString();
                    pageControl.RouteValue[property.Name]= value;
                }
            }
            var url = helper.RouteUrl(pageControl.RouteName,new RouteValueDictionary(pageControl.RouteValue));
            return new MvcHtmlString(url);
        }

}

    public static class StringExtension
    {
        /// <summary>
        /// generate a human reable slug
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public static string GenerateSlug(this string phrase)
        {
            var str = phrase;
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        /// <summary>
        /// generate a pinyin slug for a chinese string
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public static string GeneratePinYinSlug(this string phrase)
        {
            var str = Pinyin.GetPinyin(phrase);
            return str.GenerateSlug();
        } 


    }

}