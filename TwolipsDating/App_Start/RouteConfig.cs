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
                name: "SearchByTag",
                url: "search/tag/{tag}",
                defaults: new { controller = "Search", action = "Tag" }
            );

            routes.MapRoute(
                name: "SearchByUser",
                url: "search/{user}",
                defaults: new { controller = "Search", action = "Index"}
            );

            routes.MapRoute(
                name: "QuizDefaultWithSeoName",
                url: "{controller}/{action}/{id}/{seoName}",
                defaults: new { controller = "Trivia", action = "Quiz", id = UrlParameter.Optional },
                constraints: new { id = @"^\d+$" }
            );

            routes.MapRoute(
                name: "ProfileDefaultWithSeoName",
                url: "{controller}/{id}/{seoName}",
                defaults: new { controller = "Profile", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"^\d+$" }
            );

            routes.MapRoute(
                name: "ProfileDefaultWithoutAction",
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