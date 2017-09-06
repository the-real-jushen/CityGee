using System.Web;
using System.Web.Optimization;

namespace OldHouse.Web
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/jquery.form.js"));
            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js"));
            //行政区选择
            bundles.Add(new ScriptBundle("~/bundles/dist").Include(
                      "~/Scripts/distpicker.data.min.js",
                      "~/Scripts/distpicker.min.js"));
            //标签编辑
            bundles.Add(new ScriptBundle("~/bundles/tagsinput").Include(
                    "~/Scripts/jquery.tagsinput.js"));
            bundles.Add(new StyleBundle("~/Content/css/tagsinput").Include(
                "~/Content/jquery.tagsinput.css"));
            //日期选择
            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                    "~/Scripts/bootstrap-datepicker.js"));
            bundles.Add(new StyleBundle("~/Content/css/datepicker").Include(
                "~/Content/jquery.datetimepicker.css"));
            //jquery验证
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.validate*"));
            //site js
            bundles.Add(new ScriptBundle("~/bundles/business").Include(
                    "~/Scripts/amplify.core.js",
                    "~/Scripts/amplify.js",
                    "~/Scripts/amplify.request.js",
                    "~/Scripts/oldHouseScript.js",
                    "~/Scripts/wgs2mars.js",    //transform wgs84 to gcj-02
                    "~/Scripts/PagingControl.js"));
            //瀑布流
            bundles.Add(new ScriptBundle("~/bundles/freewall").Include(
                    "~/Scripts/freewall.js"));
            //basic css
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"
                      ));
            //picachoose
            bundles.Add(new StyleBundle("~/Content/css/pikachoose").Include(
                      "~/Content/base.css",
                      "~/Content/jquery.fancybox.css"));
            bundles.Add(new ScriptBundle("~/bundles/pikachoose").Include(
                "~/Scripts/jquery.jcarousel.min.js",
                "~/Scripts/jquery.pikachoose.min.js",
                "~/Scripts/jquery.touchwipe.min.js",
                "~/Scripts/jquery.fancybox.pack.js"));
            //GalleryView
            bundles.Add(new StyleBundle("~/Content/css/galleryview").Include(
                "~/Content/jquery.galleryview-3.0-dev.css"));
            bundles.Add(new ScriptBundle("~/bundles/galleryview").Include(
                "~/Scripts/jquery.timers-1.2.js",
                "~/Scripts/jquery.easing.1.3.js",
                "~/Scripts/jquery.galleryview-3.0-dev.js"));
            //file upload
            bundles.Add(new StyleBundle("~/Content/css/fileupload").Include(
                "~/Content/fileinput.css"));
            bundles.Add(new ScriptBundle("~/bundles/fileupload").Include(
                "~/Scripts/fileinput-utf8.js",
                "~/Scripts/fileinput_locale_cn-utf8.js"));
        }
    }
}
