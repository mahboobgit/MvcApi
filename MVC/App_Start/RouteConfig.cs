

namespace MVC
{
    using System.Web.Mvc;
    using System.Web.Routing;


    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{wb}",
                defaults: new { controller = "Workbook", action = "Index", wb = UrlParameter.Optional }
            );
        }
    }
}
