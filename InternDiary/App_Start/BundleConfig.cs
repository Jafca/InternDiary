using System.Web;
using System.Web.Optimization;

namespace InternDiary
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/Chosen").Include(
                      "~/Scripts/Chosen/chosen.jquery.min.js",
                      "~/Scripts/Chosen/ChosenExtra.js"));

            bundles.Add(new StyleBundle("~/Content/Chosen").Include(
                      "~/Content/Chosen/chosen.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/Countable").Include(
                      "~/Scripts/Countable/Countable.js",
                      "~/Scripts/Countable/CountableExtra.js"));
        }
    }
}
