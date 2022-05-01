using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Swagger
{
    public class CustomHeaderSwaggerAttribute : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "UserId",
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "RefreshToken",
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });

            //operation.Parameters.Add(new OpenApiParameter
            //{
            //    Name = "AccessToken",
            //    In = ParameterLocation.Header,
            //    Required = false,
            //    Schema = new OpenApiSchema
            //    {
            //        Type = "string"
            //    }
            //});
        }

    }
}
