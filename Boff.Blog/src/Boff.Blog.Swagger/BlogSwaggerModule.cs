using Boff.Blog.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Boff.Blog
{
    [DependsOn(typeof(BlogDomainModule))]
    public class BlogSwaggerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //context.Services.AddSwagger();
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            if (hostingEnvironment.IsDevelopment())
            {
                ConfigureMiniProfiler(context);
            }
            ConfigureSwaggerServices(context);
        }
        private void ConfigureMiniProfiler(ServiceConfigurationContext context)
        {
            context.Services.AddMiniProfiler(configureOptions =>
            {
                configureOptions.RouteBasePath = "/api/blog/mini-profiler-resources";
            }).AddEntityFramework();
        }

        private static void ConfigureSwaggerServices(ServiceConfigurationContext context)
        {
            context.Services.AddSwaggerGen(
                options =>
                {
                    options.EnableAnnotations();
                    options.SwaggerDoc("App", new OpenApiInfo { Title = "App API", Version = "v1",Description="用户端接口" });
                    options.SwaggerDoc("Admin", new OpenApiInfo { Title = "Admin API", Version = "v1", Description = "管理端接口" });
                    options.SwaggerDoc("Common", new OpenApiInfo { Title = "Common API", Version = "v1", Description = "通用接口" });
                    options.DocInclusionPredicate((docName, description) =>
                    {
                        if (docName == description.GroupName) return true;
                        if (description.GroupName != "App" && description.GroupName != "Admin" && docName == "Common") return true;
                        return false;
                    });

                    //授权按钮
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Description = "请输入 Bearer {Token} 进行身份验证",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                    });
                    //options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    //{
                    //    Description = "oauth2模式授权，请输入 Bearer {Token} 进行身份验证",
                    //    Name = "Authorization",
                    //    In = ParameterLocation.Header,
                    //    Type = SecuritySchemeType.ApiKey
                    //});
                    options.OperationFilter<CustomerSecurityRequirementsOperationFilter>("Bearer", new List<string> { "Role", "User", "IdentityUser", "IdentityRole", "Profile", "Permissions" });
                    options.OperationFilter<SecurityRequirementsOperationFilter>(true, "Bearer");
                    options.OperationFilter<AddResponseHeadersFilter>();
                    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();


                    options.DocumentFilter<SwaggerEnumDescriptionFilter>(new List<string> 
                    { 
                        "Boff.Blog.Application", "Boff.Blog.Application.Contracts", "Boff.Blog.Domain", "Boff.Blog.Domain.Shared",                     
                    });

                    var commentsFiles = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "*.xml")
                        .Where(f => System.IO.Path.GetFileName(f).StartsWith("Boff.Blog"));
                    foreach (var item in commentsFiles)
                    {
                        options.IncludeXmlComments(item);
                    }
                    options.CustomSchemaIds(x => x.FullName);
                    options.CustomOperationIds(x => x.ActionDescriptor.DisplayName);

                });
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            //context.GetApplicationBuilder().UseSwagger().UseSwaggerUI();
         
            var app = context.GetApplicationBuilder();

            if (context.GetEnvironment().IsDevelopment())
            {
                app.UseMiniProfiler();
            }
            app.UseSwagger(options => { options.RouteTemplate = "api/blog/swagger/{documentName}/swagger.json"; });
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "api/blog/swagger";
                options.SwaggerEndpoint("App/swagger.json", "App API");
                options.SwaggerEndpoint("Admin/swagger.json", "Admin API");
                options.SwaggerEndpoint("Common/swagger.json", "Common API");

                //// 模型的默认扩展深度，设置为 -1 完全隐藏模型
                //options.DefaultModelsExpandDepth(-1);
                //// API文档仅展开标记
                //options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

                StringBuilder stringBuilder = new StringBuilder(options.HeadContent);
                stringBuilder.Append(
                    "<script async=\"async\" id=\"mini-profiler\" src=\"/api/blog/mini-profiler-resources/includes.min.js?v=4.2.1+b27bea37e9\" data-version=\"4.2.1+b27bea37e9\" data-path=\"/api/blog/mini-profiler-resources/\" data-current-id=\"01ac5727-1006-4477-98bd-c7a3022bb5e0\" data-ids=\"f3a8e670-5bab-49f5-8676-2c09b8bf6a22,abcf28d2-6a8e-45f8-968f-a5cae1af66fc,01ac5727-1006-4477-98bd-c7a3022bb5e0\" data-position=\"Left\"\" data-scheme=\"Light\" data-authorized=\"true\" data-max-traces=\"10\" data-toggle-shortcut=\"Alt+P\" data-trivial-milliseconds=\"2.0\" data-ignored-duplicate-execute-types=\"Open,OpenAsync,Close,CloseAsync\"></script>");
                options.HeadContent = stringBuilder.ToString();
                });
        }
    }
}
