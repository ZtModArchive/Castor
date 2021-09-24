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
