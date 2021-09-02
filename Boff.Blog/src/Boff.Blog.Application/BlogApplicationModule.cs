
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;


namespace Boff.Blog
{
    [DependsOn(
        //typeof(BlogDomainModule),
        typeof(AbpIdentityApplicationModule),
         typeof(AbpAutoMapperModule)
        )]
    public class BlogApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<BlogApplicationModule>();
            });

        }
    }
}
