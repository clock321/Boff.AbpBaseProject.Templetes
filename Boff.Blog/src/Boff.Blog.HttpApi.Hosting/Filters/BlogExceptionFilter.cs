﻿using log4net;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boff.Blog.Web.Filters
{
    public class BlogExceptionFilter : IExceptionFilter
    {
        private readonly ILog _log;

        public BlogExceptionFilter()
        {
            _log = LogManager.GetLogger(typeof(BlogExceptionFilter));
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void OnException(ExceptionContext context)
        {
            // 错误日志记录
            _log.Error($"{context.HttpContext.Request.Path}|{context.Exception.Message}", context.Exception);
        }
    }
}
