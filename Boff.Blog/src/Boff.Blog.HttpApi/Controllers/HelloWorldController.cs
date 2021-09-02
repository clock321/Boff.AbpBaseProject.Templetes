using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Boff.Blog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : AbpController
    {

        [HttpGet]
        public string HelloWorld()
        {
            return "HelloWorld";
        }

        [HttpGet]
        [Route("Exception")]
        public string Exception()
        {
            throw new NotImplementedException("这是一个未实现的异常接口");
        }
    }
}
