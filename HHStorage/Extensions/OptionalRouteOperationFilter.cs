using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HHStorage.Extensions {
    public class OptionalRouteOperationFilter : IOperationFilter {
        public void Apply(Operation operation, OperationFilterContext context) {
            var action = context.ApiDescription.Properties.First().Value as ControllerActionDescriptor;
            if (action == null) return;
            foreach (ControllerParameterDescriptor parameter in action.Parameters) {
                if (!IsFromRoute(parameter)) {
                    continue;
                }


                var name = GetParameterName(parameter);

                var parameterConfig = operation.Parameters.SingleOrDefault(x => x.Name == name);


                parameterConfig.Required = context.ApiDescription.RelativePath.IndexOf("{" + name + "}") > -1;

                if (!parameterConfig.Required) {
                    operation.Parameters = operation.Parameters.Where(x => x.Name != name).ToList();
                }
            }
        }

        private string GetParameterName(ControllerParameterDescriptor parameter) {
            var routeAttr = parameter.ParameterInfo.GetCustomAttribute<FromRouteAttribute>();
            var queryAttr = parameter.ParameterInfo.GetCustomAttribute<FromQueryAttribute>();
            var headerAttr = parameter.ParameterInfo.GetCustomAttribute<FromHeaderAttribute>();
            var formAttr = parameter.ParameterInfo.GetCustomAttribute<FromFormAttribute>();

            return routeAttr?.Name ?? queryAttr?.Name ?? headerAttr?.Name ?? formAttr?.Name ?? parameter.Name;
        }

        private bool IsFromRoute(ControllerParameterDescriptor parameter) {
            return parameter.ParameterInfo.GetCustomAttribute<FromRouteAttribute>() != null;
        }
    }
}
