using System.Web.Optimization;

namespace TwolipsDating
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/raty").Include(
                    "~/Scripts/jquery.raty.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/site.css",
                    "~/Content/chosen.css,",
                    "~/Content/bootstrap-image-gallery.css"));

            bundles.Add(new ScriptBundle("~/bundles/chosen").Include(
                    "~/Scripts/chosen.jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-image-gallery").Include(
                    "~/Scripts/bootstrap-image-gallery.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}