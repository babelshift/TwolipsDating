using System.Web.Optimization;

namespace TwolipsDating
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region JavaScript

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/manage/settings").Include(
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/manage/index").Include(
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/chosen.jquery.js",
                "~/Scripts/jquery-ui.js",
                "~/Scripts/Manage/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/trivia/timed").Include(
                "~/Scripts/Trivia/timed.js"));

            bundles.Add(new ScriptBundle("~/bundles/trivia/random").Include(
                "~/Scripts/Trivia/random.js"));

            bundles.Add(new ScriptBundle("~/bundles/trivia/quiz").Include(
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/Trivia/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/tags/index").Include(
                "~/Scripts/Tags/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/store/index").Include(
                "~/Scripts/Store/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/store/cart").Include(
                "~/Scripts/Store/cart.js"));

            bundles.Add(new ScriptBundle("~/bundles/search/index").Include(
                "~/Scripts/chosen.jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/profile/create").Include(
                    "~/Scripts/jquery.validate*",
                    "~/Scripts/jquery.unobtrusive-ajax.js",
                    "~/Scripts/chosen.jquery.js",
                    "~/Scripts/jquery-ui.js",
                    "~/Scripts/Profile/create.js"));

            bundles.Add(new ScriptBundle("~/bundles/home/dashboard").Include(
                    "~/Scripts/bootstrap-image-gallery.js",
                    "~/Scripts/Home/dashboard.js"));

            bundles.Add(new ScriptBundle("~/bundles/profile/index").Include(
                    "~/Scripts/jquery.validate*",
                    "~/Scripts/jquery.unobtrusive-ajax.js",
                    "~/Scripts/bootstrap-image-gallery.js",
                    "~/Scripts/jquery.raty.js",
                    "~/Scripts/chosen.jquery.js",
                    "~/Scripts/jquery.form.min.js",
                    "~/Scripts/draggable_background.js",
                    "~/Scripts/Profile/index.js"));

            #endregion

            #region CSS

            bundles.Add(new StyleBundle("~/Content/jquery-ui").Include(
                        "~/Content/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/chosen").Include(
                    "~/Content/chosen.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap-image-gallery").Include(
                    "~/Content/bootstrap-image-gallery.css"));

            bundles.Add(new StyleBundle("~/Content/font-awesome").Include(
                    "~/Content/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/awesome-bootstrap-checkbox").Include(
                    "~/Content/font-awesome.css",
                    "~/Content/awesome-bootstrap-checkbox.css"));

            #endregion

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