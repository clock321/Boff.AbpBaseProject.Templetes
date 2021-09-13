using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace Boff.Blog
{
    [DependsOn(
          typeof(BlogDomainModule),
          typeof(AbpEntityFrameworkCoreModule),
          typeof(AbpEntityFrameworkCoreMySQLModule),
          typeof(AbpEntityFrameworkCoreSqlServerModule),
          typeof(AbpEntityFrameworkCorePostgreSqlModule),
          typeof(AbpEntityFrameworkCoreSqliteModule)
      )]
    public class BlogFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<BlogDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseMySQL();

                //switch (AppSettings.EnableDb)
                //{
                //    case "MySQL":
                //        options.UseMySQL();
                //        break;
                //    case "SqlServer":
                //        options.UseSqlServer();
                //        break;
                //    case "PostgreSql":
                //        options.UseNpgsql();
                //        break;
                //    case "Sqlite":
                //        options.UseSqlite();
                //        break;
                //    default:
                //        options.UseMySQL();
                //        break;
                //}
            });
        }
    }
}
