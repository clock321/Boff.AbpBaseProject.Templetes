using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace Boff.Blog
{
    [DependsOn(typeof(AbpIdentityDomainModule))]
    public class BlogDomainModule : AbpModule
    {

    }
}