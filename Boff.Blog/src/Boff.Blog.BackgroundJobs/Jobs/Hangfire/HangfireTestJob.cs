using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boff.Blog.BackgroundJobs.Jobs.Hangfire
{
    public class HangfireTestJob : IBackgroundJob
    {
        public async Task ExecuteAsync()
        {
            Console.WriteLine("定时任务测试");

            await Task.CompletedTask;
        }
    }
}
