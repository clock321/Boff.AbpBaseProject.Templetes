using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Boff.Blog.Swagger
{
    public class CustomerSecurityRequirementsOperationFilter : IOperationFilter
    {
        private readonly string securitySchemaName;
        private readonly string[] groupNames;

        public CustomerSecurityRequirementsOperationFilter(
            string securitySchemaName = "oauth2",
            List<string> groupNames=null)
        {
            this.securitySchemaName = securitySchemaName;
            this.groupNames = groupNames?.ToArray()??new string[0];
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var flag = this.groupNames.Contains(context.ApiDescription.GroupName);
            if(!flag)return;
            IList<OpenApiSecurityRequirement> security = operation.Security;
            OpenApiSecurityRequirement securityRequirement = new OpenApiSecurityRequirement();
            securityRequirement.Add(new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = new ReferenceType?(ReferenceType.SecurityScheme),
                    Id = this.securitySchemaName
                }
            }, Array.Empty<string>());
            security.Add(securityRequirement);
        }
    }
    
}
