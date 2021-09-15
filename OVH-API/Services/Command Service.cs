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
                        Console.WriteLine("\nlogin\nfetch ips\nmonitor\nquit\n");
                        break;
                    case "login":
                        if (!Utilities.Login())
                            Console.WriteLine("failed to login");
                            break;
                    case "fetch ips":
                        await Utilities.FetchIPs();
                        break;
                    case "monitor":
                        if (Utilities.Login())
                            Utilities.Derp();
                        else
                            Console.WriteLine("failed to login");
                        break;
                    default:
                        Console.WriteLine("Error, command doesn't exist, please try again. Type 'Help' to view the available commands.");
                        break;
                }
            }
        }
    }
}
