using Castor3.Interfaces;
using Castor3.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Castor3
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            CastorApp app = serviceProvider.GetService<CastorApp>();
            app.Run(args);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<CastorApp>();
            services.AddScoped<IZippingService, ZippingService>();
            services.AddScoped<ICommandService, CommandService>();
        }
    }
}
