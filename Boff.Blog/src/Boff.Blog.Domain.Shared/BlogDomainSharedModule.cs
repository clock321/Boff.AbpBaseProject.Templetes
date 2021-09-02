

using Volo.Abp.Identity;
using Volo.Abp.Modularity;


namespace Boff.Blog
{
    [DependsOn(typeof(AbpIdentityDomainSharedModule))]
    public class BlogDomainSharedModule : AbpModule
    {
    }

}
