
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace Boff.Blog
{
    [DependsOn(
          typeof(AbpIdentityHttpApiModule)
        , typeof(BlogApplicationModule)
      )]
    public class BlogHttpApiModule : AbpModule
    {

    }

    public abstract class BlogControllerBase : AbpController
    {
    }

    [ApiExplorerSettings(GroupName = "App")]
    [Authorize]
    public abstract class BlogAppController : BlogControllerBase
    {

    }

    [ApiExplorerSettings(GroupName = "Admin")]
    [Authorize]
    public abstract class BlogAdminController : BlogControllerBase
    {

    }
}
