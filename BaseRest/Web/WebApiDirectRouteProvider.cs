using BaseRest.Extensions;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace BaseRest.Web
{

    public class WebApiDirectRouteProvider : DefaultDirectRouteProvider
    {
        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            List<IDirectRouteFactory> results = new List<IDirectRouteFactory>();

            var attributes = actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(inherit: true);
            foreach (IDirectRouteFactory att in attributes)
            {
                results.Add(GetAttributeWithControllerLiteralReplaced(att, actionDescriptor));
            }

            return results;
        }

        private static IDirectRouteFactory GetAttributeWithControllerLiteralReplaced(IDirectRouteFactory att, HttpActionDescriptor actionDescriptor)
        {
            IDirectRouteFactory result = att;

            var routeAttribute = (att as RouteAttribute);
            if (routeAttribute != null && routeAttribute.Template.Contains("{controller}"))
            {
                string controller = actionDescriptor.ControllerDescriptor.ControllerName.CamelCaseToSplitLower();
                result = new RouteAttribute(routeAttribute.Template.Replace("{controller}", controller));
            }

            return result;
        }
    }
}
