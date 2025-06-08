using System.Text;
using System.Web.Http;

namespace Kursovaya
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            config.Formatters.JsonFormatter.SupportedEncodings.Clear();
            config.Formatters.JsonFormatter.SupportedEncodings.Add(new UTF8Encoding(false));

            // Атрибутные маршруты API
            config.MapHttpAttributeRoutes();

            // При необходимости — fallback маршрут
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
