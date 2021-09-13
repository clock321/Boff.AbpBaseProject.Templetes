using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace Boff.Blog.Application.Caching
{
    [DependsOn(
        typeof(AbpCachingModule)
        ,typeof(BlogDomainModule)
    // ...
    )]
    public class BlogApplicationCachingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            context.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = AppSettings.Caching.RedisConnectionString;
                //options.InstanceName
                //options.ConfigurationOptions
            });
        }
    }
}