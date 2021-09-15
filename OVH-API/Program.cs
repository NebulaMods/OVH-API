using System;
using System.Threading.Tasks;

namespace OVHAPI
{
    class Program
    {
        static async Task Main(string[] arguments) => await new Services.Startup(arguments).RunAsync();
    }
}
