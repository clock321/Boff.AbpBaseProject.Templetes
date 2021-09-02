using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Boff.Blog
{
    public class BlogAppServiceBase: ApplicationService
    {
    }


    [ApiExplorerSettings(GroupName = "Admin")]
    [Authorize]
    public abstract class BlogAdminAppService : BlogAppServiceBase
    {

    }


    [ApiExplorerSettings(GroupName = "App")]
    public abstract class BlogAppAppService : BlogAppServiceBase
    {

    }
}



