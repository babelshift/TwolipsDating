using System.Web.Optimization;

namespace TwolipsDating
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region JavaScript

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/message/conversation").Include(
                "~/Scripts/Shared/followify.js",
                "~/Scripts/Message/conversation.js",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/manage/settings").Include());

            bundles.Add(new ScriptBundle("~/bundles/manage/index").Include(
                "~/Scripts/chosen.jquery.js",
                "~/Scripts/jquery-ui.js",
                "~/Scripts/Manage/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/trivia/timed").Include(
                "~/Scripts/Shared/followify.js",
                "~/Scripts/Trivia/timed.js"));

            bundles.Add(new ScriptBundle("~/bundles/trivia/random").Include(
                "~/Scripts/Shared/followify.js",
                "~/Scripts/Trivia/random.js"));

            bundles.Add(new ScriptBundle("~/bundles/trivia/quiz").Include(
                "~/Scripts/Shared/followify.js",
                "~/Scripts/Trivia/quiz.js"));

            bundles.Add(new ScriptBundle("~/bundles/tags/index").Include(
                "~/Scripts/Tags/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/store/index").Include(
                "~/Scripts/Shared/followify.js",
                "~/Scripts/Store/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/store/cart").Include(
                "~/Scripts/Store/cart.js"));

            bundles.Add(new ScriptBundle("~/bundles/search/index").Include(
                "~/Scripts/Shared/followify.js",
                "~/Scripts/Search/index.js",
                "~/Scripts/chosen.jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/profile/create").Include(
                    "~/Scripts/chosen.jquery.js",
                    "~/Scripts/jquery-ui.js",
                    "~/Scripts/Profile/create.js"));

            bundles.Add(new ScriptBundle("~/bundles/home/dashboard").Include(
                    "~/Scripts/bootstrap-image-gallery.js",
                    "~/Scripts/Shared/followify.js",
                    "~/Scripts/Home/dashboard.js"));

            bundles.Add(new ScriptBundle("~/bundles/profile/index").Include(
                    "~/Scripts/bootstrap-image-gallery.js",
                    "~/Scripts/jquery.raty.js",
                    "~/Scripts/chosen.jquery.js",
                    "~/Scripts/jquery.form.min.js",
                    "~/Scripts/draggable_background.js",
                    "~/Scripts/Shared/followify.js",
                    "~/Scripts/jquery.cookie.js",
                    "~/Scripts/Profile/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/shared/layout").Include(
                    "~/Scripts/Shared/layout.js"));

            bundles.Add(new ScriptBundle("~/bundles/profile/quick").Include(
                    "~/Scripts/Shared/followify.js",
                    "~/Scripts/Profile/quick.js"));

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