using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Boff.Blog.Swagger
{
    public class SwaggerEnumDescriptionFilter : IDocumentFilter
    {
        private readonly string[] _assemblyNames;

        public SwaggerEnumDescriptionFilter(List<string> assemblyNames)
        {
            _assemblyNames = assemblyNames?.ToArray() ?? new string[0];
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // add enum descriptions to result models
            foreach (var property in swaggerDoc.Components.Schemas.Where(x => x.Value?.Enum?.Count > 0))
            {
                IList<IOpenApiAny> propertyEnums = property.Value.Enum;
                if (propertyEnums != null && propertyEnums.Count > 0)
                {
                    property.Value.Description += ": " + DescribeEnum(propertyEnums, property.Key);
                }
            }

            // add enum descriptions to input parameters
            foreach (var pathItem in swaggerDoc.Paths)
            {
                DescribeEnumParameters(pathItem.Value.Operations, swaggerDoc, context.ApiDescriptions, pathItem.Key);
            }
        }

        private void DescribeEnumParameters(IDictionary<OperationType, OpenApiOperation> operations, OpenApiDocument swaggerDoc, IEnumerable<ApiDescription> apiDescriptions, string path)
        {
            path = path.Trim('/');
            if (operations != null)
            {
                var pathDescriptions = apiDescriptions.Where(a => a.RelativePath == path);
                foreach (var oper in operations)
                {
                    var operationDescription = pathDescriptions.FirstOrDefault(a => a.HttpMethod.Equals(oper.Key.ToString(), StringComparison.InvariantCultureIgnoreCase));
                    foreach (var param in oper.Value.Parameters)
                    {
                        var parameterDescription = operationDescription.ParameterDescriptions.FirstOrDefault(a => a.Name == param.Name);
                        if (parameterDescription != null && TryGetEnumType(parameterDescription.Type, out Type enumType))
                        {
                            var paramEnum = swaggerDoc.Components.Schemas.FirstOrDefault(x => x.Key == enumType.Name||x.Key==enumType.FullName);
                            if (paramEnum.Value != null)
                            {
                                param.Description = paramEnum.Value.Description;
                            }
                        }
                    }
                }
            }
        }

        bool TryGetEnumType(Type type, out Type enumType)
        {
            if (type.IsEnum)
            {
                enumType = type;
                return true;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                if (underlyingType != null && underlyingType.IsEnum == true)
                {
                    enumType = underlyingType;
                    return true;
                }
            }
            else
            {
                Type underlyingType = GetTypeIEnumerableType(type);
                if (underlyingType != null && underlyingType.IsEnum)
                {
                    enumType = underlyingType;
                    return true;
                }
                else
                {
                    var interfaces = type.GetInterfaces();
                    foreach (var interfaceType in interfaces)
                    {
                        underlyingType = GetTypeIEnumerableType(interfaceType);
                        if (underlyingType != null && underlyingType.IsEnum)
                        {
                            enumType = underlyingType;
                            return true;
                        }
                    }
                }
            }

            enumType = null;
            return false;
        }

        Type GetTypeIEnumerableType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var underlyingType = type.GetGenericArguments()[0];
                if (underlyingType.IsEnum)
                {
                    return underlyingType;
                }
            }

            return null;
        }

        private Type GetEnumTypeByName(string enumTypeName)
        {
            List<Type> types = new List<Type>();
            foreach (var assemblyName in _assemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);
                types.AddRange(assembly.GetTypes());
            }

            return types.FirstOrDefault(x => x.Name == enumTypeName||x.FullName==enumTypeName);
        }

        private string DescribeEnum(IList<IOpenApiAny> enums, string proprtyTypeName)
        {
            var type = GetEnumTypeByName(proprtyTypeName);
            if (type == null)
                return null;

            var enumDescriptions = new List<string>();
            foreach (OpenApiInteger item in enums)
            {
                var value = Enum.Parse(type, item.Value.ToString());
                var desc = GetDescription(type, value);

                if (string.IsNullOrEmpty(desc))
                    enumDescriptions.Add($"{item.Value.ToString()}:{Enum.GetName(type, value)}; ");
                else
                    enumDescriptions.Add($"{item.Value.ToString()}:{Enum.GetName(type, value)},{desc}; ");

            }
            return $"<br/>{Environment.NewLine}{string.Join("<br/>" + Environment.NewLine, enumDescriptions)}";
            
        }


        private string GetDescription(Type t, object value)
        {
            foreach (MemberInfo mInfo in t.GetMembers())
            {
                if (mInfo.Name == t.GetEnumName(value))
                {
                    foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                    {
                        if (attr.GetType() == typeof(DescriptionAttribute))
                        {
                            return ((DescriptionAttribute)attr).Description;
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}