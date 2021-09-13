using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.FileProviders;
using System;
using Microsoft.Extensions.Hosting;

namespace Boff.Blog
{
    public static class AppEnv
    {
        private static bool _hasInit = false;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void InitAppEnvironment(IHostEnvironment env)
        {
            if (_hasInit)
            {
                throw new Exception("GlobalEnvironment Has Inited");
            }
            if (env.IsDevelopment())
            {
                IsDevelopmentEnvironment = true;
            }

            ApplicationName = env.ApplicationName;
            ContentRootFileProvider = env.ContentRootFileProvider;
            ContentRootPath = env.ContentRootPath;
            EnvironmentName = env.EnvironmentName;
            _hasInit = true;
        }

        public static bool IsDevelopmentEnvironment { get; private set; } = false;


        public static string Directory => Environment.CurrentDirectory;


        public static string ApplicationName { get; private set; }


        public static IFileProvider ContentRootFileProvider { get; private set; }

        public static string ContentRootPath { get; private set; }


        public static string EnvironmentName { get; private set; }

      
    }
}
