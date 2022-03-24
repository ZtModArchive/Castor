using Castor.Interfaces;
using Castor.Services;
using Castoreum.Compression;
using Castoreum.Config.Models;
using Castoreum.Config.Service;
using Castoreum.Installation;
using Castoreum.Interface.Service.Compression;
using Castoreum.Interface.Service.Config;
using Castoreum.Interface.Service.Installation;
using Castoreum.Interface.Service.Watch;
using Castoreum.Watch;
using Microsoft.Extensions.DependencyInjection;

namespace Castor
{
    static class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider.GetService<CastorApp>();
            CastorApp.Run(args);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<CastorApp>();
            services.AddScoped<ICommandService, CommandService>();
            services.AddScoped<ICompressionManager, CompressionManager>();
            services.AddScoped<IConfig, CastorConfig>();
            services.AddScoped<IConfigManager, ConfigManager>();
            services.AddScoped<IInstallationManager, InstallationManager>();
            services.AddScoped<IProcessWatcher, ProcessWatcher>();
        }
    }
}
