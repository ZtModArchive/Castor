using Castor.Interfaces;
using Castor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Castor
{
    class Program
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
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IInstallerService, InstallerService>();
            services.AddScoped<IZippingService, ZippingService>();
        }
    }
}
