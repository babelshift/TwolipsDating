using System.Web.Mvc;
using System.Web.Routing;

namespace TwolipsDating
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "DefaultWithSeoName",
                url: "{controller}/{id}/{seoName}",
                defaults: new { controller = "Profile", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"^\d+$" }
            );

            routes.MapRoute(
                name: "DefaultWithoutAction",
                url: "{controller}/{id}",
                defaults: new { controller = "Profile", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"^\d+$" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}