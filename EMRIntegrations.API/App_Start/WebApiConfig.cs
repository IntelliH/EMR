using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EMRIntegrations
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
                name: "PostHL7",
                routeTemplate: "api/{controller}/PostHL7",
                defaults: new { id = RouteParameter.Optional, Controller = "IntelliHEndPoint", Action = "PostHL7" }
            );

            config.Routes.MapHttpRoute(
                name: "NetSmartPostHL7",
                routeTemplate: "api/{controller}/NetSmartPostHL7",
                defaults: new { id = RouteParameter.Optional, Controller = "IntelliHEndPoint", Action = "NetSmartPostHL7" }
                );
        }
    }
}
