using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boff.Blog.Migrations
{
    public class MeowvBlogMigrationsDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
    {
        public BlogDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();
       
            Console.WriteLine($"ConnectionString:{configuration.GetConnectionString("Default")}");           
            var builder = new DbContextOptionsBuilder<BlogDbContext>()
                .UseMySql(configuration.GetConnectionString("Default"),
                               //serverVersion: ServerVersion.AutoDetect(connectionString: configuration.GetConnectionString("Default")),
                               serverVersion: ServerVersion.Parse("8.0.18"),
                opt => {
                    opt.CommandTimeout(60);
                    opt.EnableRetryOnFailure(10);
                });

            return new BlogDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var basepath = Directory.GetCurrentDirectory();
            if (!basepath.EndsWith(".EntityFrameworkCore"))
            {
                var oldEndPath = Path.GetFileNameWithoutExtension(basepath);
                basepath = basepath.Substring(0,basepath.LastIndexOf(oldEndPath)) + "Boff.Blog.EntityFrameworkCore";
            }
            //Console.WriteLine($"CurrentDirectory:{basepath}");
            var builder = new ConfigurationBuilder()
                .SetBasePath(basepath)
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
