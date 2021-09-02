using Boff.Blog.BackgroundJobs;
using Boff.Blog.Domain.Configurations;
using Boff.Blog.ToolKits.Extensions;
using Boff.Blog.Web.Filters;
using Boff.Blog.Web.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Boff.Blog.HttpApi.Hosting
{
    [DependsOn(
          typeof(AbpAspNetCoreMvcModule),
          typeof(AbpAutofacModule),
          typeof(BlogHttpApiModule),
          typeof(BlogSwaggerModule),
          typeof(BlogFrameworkCoreModule),
          typeof(BlogBackgroundJobsModule)
       )]
    public class BlogHttpApiHostingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
          
            base.ConfigureServices(context);

            //context.Services.AddRouting(options =>
            //{
            //    // 设置URL为小写
            //    options.LowercaseUrls = true;
            //    // 在生成的URL后面添加斜杠
            //    options.AppendTrailingSlash = true;
            //});

            ConfigureConventionalControllers();

            Configure<MvcOptions>(options =>
            {
                var filterMetadata = options.Filters.FirstOrDefault(x => x is ServiceFilterAttribute attribute && attribute.ServiceType.Equals(typeof(AbpExceptionFilter)));

                // 移除 AbpExceptionFilter
                options.Filters.Remove(filterMetadata);

                // 添加自己实现的 MeowvBlogExceptionFilter
                options.Filters.Add(typeof(BlogExceptionFilter));
            });
            // 身份验证
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidateLifetime = true,
                           ClockSkew = TimeSpan.FromSeconds(30),
                           ValidateIssuerSigningKey = true,
                           ValidAudience = AppSettings.JWT.Domain,
                           ValidIssuer = AppSettings.JWT.Domain,
                           IssuerSigningKey = new SymmetricSecurityKey(AppSettings.JWT.SecurityKey.GetBytes())
                       };

                       // 应用程序提供的对象，用于处理承载引发的事件，身份验证处理程序
                       options.Events = new JwtBearerEvents
                       {
                           OnChallenge = async context =>
                           {
                               // 跳过默认的处理逻辑，返回下面的模型数据
                               context.HandleResponse();
                               context.Response.ContentType = "application/json;charset=utf-8";
                               context.Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;
                               await context.Response.WriteAsync(new Volo.Abp.Http.RemoteServiceErrorResponse(new Volo.Abp.Http.RemoteServiceErrorInfo("UnAuthorized",code:"401")).ToJson());
                           }
                       };
                   });

            // 认证授权
            context.Services.AddAuthorization();

            // Http请求
            context.Services.AddHttpClient();
        }


        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.UseV3UrlStyle = true;
                options.ConventionalControllers
                    .Create(typeof(BlogApplicationModule).Assembly, opts =>
                    {
                        opts.RootPath = "blog/app";
                        opts.TypePredicate = type => { return typeof(BlogAppAppService).IsAssignableFrom(type); };
                        opts.ControllerModelConfigurer = (conrtollerModel) =>
                        {
                            if (conrtollerModel.ControllerName.EndsWith("App"))
                                conrtollerModel.ControllerName =conrtollerModel.ControllerName.Substring(0,conrtollerModel.ControllerName.Length - 3);
                        };
                        opts.UseV3UrlStyle = true;
                        opts.UrlActionNameNormalizer = (name) => { return name.Action.ActionName; };
                    })
                    .Create(typeof(BlogApplicationModule).Assembly, opts =>
                    {
                        opts.RootPath = "blog/admin";
                        opts.TypePredicate = type => { return typeof(BlogAdminAppService).IsAssignableFrom(type); };
                        opts.ControllerModelConfigurer = (conrtollerModel) =>
                        {
                            if (conrtollerModel.ControllerName.EndsWith("Admin"))
                                conrtollerModel.ControllerName = conrtollerModel.ControllerName.Substring(0, conrtollerModel.ControllerName.Length - 5);
                        };
                        opts.UseV3UrlStyle = true;
                        opts.UrlActionNameNormalizer = (name) => { return name.Action.ActionName; };
                    })
                    .Create(typeof(BlogApplicationModule).Assembly, opts =>
                    {
                        opts.RootPath = "blog";
                        opts.TypePredicate = type => { return typeof(BlogAppServiceBase).IsAssignableFrom(type) && !typeof(BlogAppAppService).IsAssignableFrom(type) && !typeof(BlogAdminAppService).IsAssignableFrom(type); };
                        opts.UseV3UrlStyle = true;
                        opts.UrlActionNameNormalizer = (name) => { return name.Action.ActionName; };
                    })
                    .Create(typeof(BlogHttpApiModule).Assembly,opts=>
                    {
                        opts.UseV3UrlStyle = true;
                        opts.UrlActionNameNormalizer = (name) => { return name.Action.ActionName; };
                    })
                    ;
 
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            // 环境变量，开发环境
            if (env.IsDevelopment())
            {
                // 生成异常页面
                app.UseDeveloperExceptionPage();
            }
            // 使用HSTS的中间件，该中间件添加了严格传输安全头
            app.UseHsts();

            // 转发将标头代理到当前请求，配合 Nginx 使用，获取用户真实IP
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // 路由
            app.UseRouting();

            // 跨域
            app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            // 异常处理中间件
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            // 身份验证
            app.UseAuthentication();

            // 认证授权
            app.UseAuthorization();

            // HTTP => HTTPS
            //app.UseHttpsRedirection();

            // 路由映射
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.UseConfiguredEndpoints();
        }
    }
}