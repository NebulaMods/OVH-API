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
        #region Look & Feel

        //[DllImport("kernel32.dll", ExactSpelling = true)]
        //private static extern IntPtr GetConsoleWindow();

        //[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        //private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        #endregion
        public Startup(string[] arguments)
        {
            Console.Title = "Nebula Mods Inc. [OVH API] ~ Nebula";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //SetWindowPos(GetConsoleWindow(), 0, 550, 300, 0, 0, 0x0001);
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
                //
                await Task.Delay(-1);
            }
            catch (Exception error)
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
