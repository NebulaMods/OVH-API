using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OVHAPI.Services
{
    public class CommandService
    {
        private readonly DatabaseService _database;
        public CommandService(DatabaseService database)
        {
            _database = database;
        }
        public async Task StartAsync()
        {
            Console.WriteLine("Please enter a command :)");

            while (true)
            {
                string Input;
                switch (Input = Console.ReadLine().ToLower())
                {
                    case "quit":
                        Environment.Exit(0);
                        break;
                    case "help":
                        Console.WriteLine("\nLogin\nFetch ips\nMonitor\nQuit\n");
                        break;
                    case "login":
                        if (!Utilities.Login())
                            Console.WriteLine("Failed to login");
                            break;
                    case "Fetch IPs":
                        await Utilities.FetchIPs();
                        break;
                    case "Monitor":
                        if (Utilities.Login())
                            Utilities.Detection();
                        else
                            Console.WriteLine("Failed to login");
                        break;
                    default:
                        Console.WriteLine("Error, command doesn't exist, please try again. Type 'Help' to view the available commands.");
                        break;
                }
            }
        }
    }
}
