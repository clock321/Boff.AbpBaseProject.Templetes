using Boff.Blog.Core.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Boff.Blog.Web.Middleware
{
    /// <summary>
    /// 异常处理中间件
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await ExceptionHandlerAsync(context, ex.Message,500);
            }
            finally
            {
                var statusCode = context.Response.StatusCode;
                if (statusCode != StatusCodes.Status200OK)
                {
                    Enum.TryParse(typeof(HttpStatusCode), statusCode.ToString(), out object message);
                    await ExceptionHandlerAsync(context, message.ToString(), statusCode);
                }
            }
        }

        /// <summary>
        /// 异常处理，返回JSON
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task ExceptionHandlerAsync(HttpContext context, string message,int StatusCode)
        {
            context.Response.ContentType = "application/json;charset=utf-8";
            await context.Response.WriteAsync(new Volo.Abp.Http.RemoteServiceErrorResponse(new Volo.Abp.Http.RemoteServiceErrorInfo(message, code: StatusCode.ToString())).ToJson());

        }
    }
}
