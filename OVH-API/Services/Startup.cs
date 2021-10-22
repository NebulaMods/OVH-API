using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OVHAPI.Services
{
    public class Startup
    {
        public Startup(string[] arguments)
        {
            Console.Title = "Nebula Mods Inc. [OVH API] ~ Nebula";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(85, 20);
                Console.SetBufferSize(85, 20);
            }
        }
        public static async Task RunAsync(string[] arguments) => await new Startup(arguments).RunAsync();

        public async Task RunAsync()
        {
            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                var provider = services.BuildServiceProvider();
                provider.GetRequiredService<DatabaseService>().Database.Migrate();
                await provider.GetRequiredService<CommandService>().StartAsync();
                await Task.Delay(-1);
            }
            catch (Exception)
            {
                
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DatabaseService>()
            .AddSingleton(new Random())
            .AddSingleton<CommandService>();
        }

    }
}
