using System.Web.Http;
using System;
using S10_00_AspnetWebApi480SdkProject;
//using Owin;

namespace S10_00_AspnetWebApi480SdkProject
{
    public class Startup
    {
        //public void Configuration(IAppBuilder app)
        //{
        //    // Create and configure Web API
        //    HttpConfiguration config = new HttpConfiguration();

        //    // Enable attribute routing or set up default routing
        //    config.MapHttpAttributeRoutes();
        //    config.Routes.MapHttpRoute(
        //        name: "DefaultApi",
        //        routeTemplate: "api/{controller}/{id}",
        //        defaults: new { id = RouteParameter.Optional }
        //    );

        //    // Additional Web API configuration can occur here

        //    // Integrate Web API with the OWIN pipeline
        //    app.UseWebApi(config);
        //}
        public void Configuration(HttpConfiguration config)
        {
            WebApiConfig.Register(config);
            //// Configure Web API routes
            //config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
