//AppSettings.cs
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace Boff.Blog
{

    public class AppSettings
    {
        private static IConfigurationRoot _config;

        public static bool Initialized => _config != null;

        public static IConfiguration Configuration
        {
            get
            {
                InitializeIfNot();

                return _config;
            }
        }

        private static void InitializeIfNot()
        {
            if (!Initialized)
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                Initialize(environment);
            }
        }

        /// <summary>
        /// Initialize configuration with appsettings.json & appsettings.{AppEnv.EnvironmentName}.json
        /// </summary>
        public static void Initialize()
        {
            //Initialize(string.Empty);
            Initialize(AppEnv.EnvironmentName);
        }

        /// <summary>
        /// Initialize configuration with appsettings.json & appsettings.{environmentName}.json
        /// </summary>
        /// <param name="environmentName"></param>
        public static void Initialize(string environmentName)
        {
            if (Initialized)
            {
                return;
            }
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appsettings.json", true, true);

            if (!string.IsNullOrEmpty(environmentName))
            {
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false);
            }
            builder.AddEnvironmentVariables();
            _config = builder.Build();
        }


        /// <summary>
        /// EnableDb
        /// </summary>
        //public static string EnableDb => _config["ConnectionStrings:Enable"];

        /// <summary>
        /// ConnectionStrings
        /// </summary>
        public static string ConnectionStrings => _config.GetConnectionString("BlogDb");


        /// <summary>
        /// GitHub
        /// </summary>
        public static class GitHub
        {
            public static int UserId => System.Convert.ToInt32(_config["Github:UserId"]);

            public static string Client_ID => _config["Github:ClientID"];

            public static string Client_Secret => _config["Github:ClientSecret"];

            public static string Redirect_Uri => _config["Github:RedirectUri"];

            public static string ApplicationName => _config["Github:ApplicationName"];
        }

        public static class JWT
        {
            public static string Domain => _config["JWT:Domain"];

            public static string SecurityKey => _config["JWT:SecurityKey"];

            public static int Expires => Convert.ToInt32(_config["JWT:Expires"]);
        }

        /// <summary>
        /// Caching
        /// </summary>
        public static class Caching
        {
            /// <summary>
            /// RedisConnectionString
            /// </summary>
            public static string RedisConnectionString => _config["Caching:RedisConnectionString"];
        }


        /// <summary>
        /// Hangfire
        /// </summary>
        public static class Hangfire
        {
            public static string Login => _config["Hangfire:Login"];

            public static string Password => _config["Hangfire:Password"];
        }
    }
}




