using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HHStorage.Extensions {
    public class FormFileOperationFilter : IOperationFilter {
        // TODO: Support ICollection<IFormFile>

        private const string formDataMimeType = "multipart/form-data";
        private static readonly string[] formFilePropertyNames =
            typeof(IFormFile).GetTypeInfo().DeclaredProperties.Select(p => p.Name).ToArray();

        public void Apply(Operation operation, OperationFilterContext context) {
            foreach (ControllerParameterDescriptor file in context.ApiDescription.ActionDescriptor.Parameters.Where(x => x.ParameterType == typeof(IFormFile))) {
                var parameter = operation.Parameters.SingleOrDefault(x => x.Name == file.Name) as NonBodyParameter;
                if (parameter == null) continue;
                parameter.In = "formData";
                parameter.Type = "file";
            }
        }
    }
}
