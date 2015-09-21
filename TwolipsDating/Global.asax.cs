using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TwolipsDating
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.Initialize();
        }

        protected void Application_BeginRequest(object sender, System.EventArgs e)
        {
            System.Web.Helpers.AntiForgeryConfig.RequireSsl = this.Request.IsSecureConnection;
        }

        protected void Application_PreSendRequestHeaders()
        {
            if (this.Request.IsSecureConnection)
            {
                this.Response.AppendHeader("Strict-Transport-Security", "max-age=31536000");
            }
        }
    }
}