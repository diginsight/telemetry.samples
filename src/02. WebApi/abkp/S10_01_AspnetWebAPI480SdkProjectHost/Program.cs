using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace S10_01_AspnetWebAPI480SdkProjectHost;

public class Startup
{
    static void Main(string[] args)
    {
        // Define the base address for your self-hosted Web API.
        string baseAddress = "http://localhost:9000/";

        // Start OWIN host 
        using (WebApp.Start<Startup>(url: baseAddress))
        {
            Console.WriteLine("Web API is running on " + baseAddress);
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }
    }

    // This method is used by OWIN at runtime.
    public void Configuration(IAppBuilder app)
    {
        // Create an instance of the HttpConfiguration class.
        HttpConfiguration config = new HttpConfiguration();

        // Register Web API routes.
        config.MapHttpAttributeRoutes(); // If you use attribute routing.

        // Alternatively, you can define a default route:
        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );

        // Optionally, you can add additional configuration such as formatters, message handlers, etc.
        // For example: config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

        // Finally, register the Web API middleware with OWIN.
        app.UseWebApi(config);
    }
}
