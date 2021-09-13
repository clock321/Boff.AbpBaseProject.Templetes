
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boff.Blog.HttpApi.Hosting
{
    public class Startup
    {

        private readonly IConfiguration _appConfiguration;
        private readonly IWebHostEnvironment env;

        public Startup(IWebHostEnvironment env, IConfiguration appConfiguration)
        {
            this.env = env;
            AppEnv.InitAppEnvironment(this.env);
            _appConfiguration = appConfiguration;
            AppSettings.Initialize();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<BlogHttpApiHostingModule>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.InitializeApplication();
        }
    }
}
