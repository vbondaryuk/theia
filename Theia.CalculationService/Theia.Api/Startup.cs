﻿using System.Web.Http;
using DryIoc;
using DryIoc.WebApi;
using Owin;
using Theia.Core.Calculations;
using Theia.Core.Services;
using Theia.Infrastructure.Calculation;
using Theia.Infrastructure.Rules;
using Theia.Services.ObjectBuilders;
using Theia.Services.SourceCodeBuilders;

namespace Theia.Api
{
    public sealed class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var httpConfiguration = new HttpConfiguration()
            .ConfigureRouting()
            .ConfigureDependencyInjection();

            appBuilder.UseWebApi(httpConfiguration);
            appBuilder.UseWebApi(httpConfiguration);
        }
        
    }

    static class Configuration
    {
        public static HttpConfiguration ConfigureRouting(this HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();

            httpConfiguration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            return httpConfiguration;
        }
        public static HttpConfiguration ConfigureDependencyInjection(this HttpConfiguration httpConfiguration)
        {
            var container = new Container();

            container.Register<IRulesCalculation, RulesCalculation>(Reuse.InWebRequest);
            container.Register<IObjectBuilder, ObjectBuilder>(Reuse.InWebRequest);
            container.Register<ISourceCodeBuilder, JsonSourceCodeBuilder>(Reuse.InWebRequest);
            container.Register<ICalculationService, JsonCalculationServiceAdapter>(Reuse.InWebRequest);
            container.Register<IRuleMapper, RuleMapper>(Reuse.InWebRequest);
            container.WithWebApi(httpConfiguration);

            return httpConfiguration;
        }
    }
}