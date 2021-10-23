using Ovh.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Discord;
using Discord.Webhook;
using System.Reflection;
using NetTools;

namespace OVHAPI
{
    public class Utilities
    {
        private static Client OVHCloud;
        public static string Profile;

        private static bool ProfileCreated()
        {
            var database = new Services.DatabaseService();
            switch (database.Setttings.Count())
            {
                case 0:
                    Console.WriteLine("Please visit this website to create the credentials needed to use the OVH API.\nhttps://ca.api.ovh.com/createToken/ = Canada\nhttps://eu.api.ovh.com/createToken/ = Europe\nhttps://api.us.ovhcloud.com/createToken/ = United States\n");
                    Console.Write("Please enter a profile name: ");
                    Profile = Console.ReadLine();
                    Console.Write("Please enter the api endpoint location ovh-ca, ovh-eu, or ovh-us: ");
                    var endpoint = Console.ReadLine();
                    Console.Write("Please enter application key: ");
                    var appkey = Console.ReadLine();
                    Console.Write("Please enter application secret: ");
                    var appsecret = Console.ReadLine();
                    Console.Write("Please enter consumer key: ");
                    var consumerkey = Console.ReadLine();
                    Console.Write("Please enter discord webhook url: ");
                    database.Setttings.Add(new Database.Settings
                    {
                        ProfileName = Profile,
                        EndPoint = endpoint,
                        ApplicationKey = appkey,
                        ApplicationSecret = appsecret,
                        ConsumerKey = consumerkey,
                        DiscordWebHookUrl = Console.ReadLine()
                    });
                    database.SaveChanges();
                    //Console.WriteLine("Then fill out the settings in the OVH-API.db file");
                    return false;
                case 1:
                    var ProfileEntry = database.Setttings.ToArray()[0];
                    Profile = ProfileEntry.ProfileName;
                    return true;
                default:
                    Console.WriteLine("Please select a profile next time.");
                    return false;

            }
        }
        public static bool Login()
        {
            bool Result = false;
            try
            {
                if (ProfileCreated())
                {
                    var database = new Services.DatabaseService();
                    var entry = database.Setttings.FirstOrDefault(x => x.ProfileName == Profile);
                    Console.WriteLine("Starting OVH API, attempting to sign into OVH.");
                    OVHCloud = new Client(entry.EndPoint, entry.ApplicationKey, entry.ApplicationSecret, entry.ConsumerKey);
                    Console.WriteLine("Successfully Signed into OVH.");
                    return Result = true;
                }
                else
                {
                    if (Profile != null)
                    {
                        var database = new Services.DatabaseService();
                        var entry = database.Setttings.FirstOrDefault(x => x.ProfileName == Profile);
                        Console.WriteLine("Starting OVH API, attempting to sign into OVH.");
                        OVHCloud = new Client(entry.EndPoint, entry.ApplicationKey, entry.ApplicationSecret, entry.ConsumerKey);
                        Console.WriteLine("Successfully Signed into OVH.");
                        FetchIPs();
                        return Result = true;
                    }
                }
                return Result;
            }
            catch
            {
                return Result;
            }
        }

        private class IPInfo
        {
            public OVHName routedTo { get; set; }
            public string ip { get; set; }
            public bool canBeTerminated { get; set; }
            public string organisationId { get; set; }
            public string country { get; set; }
            public string type { get; set; }
            public string description { get; set; }
        }
        private class OVHName
        {
            public string serviceName { get; set; }
        }

        public static Task FetchIPs()
        {
            try
            {
                if (OVHCloud != null)
                {
                    Console.WriteLine("Attempting to fetch all IPv4 IP Addresses, please wait...");
                    var IPs = OVHCloud.GetAsync("/ip").Result.MakeIPAddress().Split(',');
                    var database = new Services.DatabaseService();
                    foreach (var IP in IPs)
                    {

                        var newip = IP.Split('/')[0];
                        var entry = database.IPs.Find(IPAddress.Parse(newip));
                        if (entry == null && Uri.CheckHostName(newip) != UriHostNameType.IPv6)
                        {
                            if (IP.Split('/')[1] == "32")
                            {
                                var Info = JsonSerializer.Deserialize<IPInfo>(OVHCloud.GetAsync($"/ip/{newip}").Result);
                                database.IPs.Add(new Database.IPSchema
                                {
                                    IP = IPAddress.Parse(newip),
                                    Server = string.IsNullOrWhiteSpace(Info.routedTo.serviceName) ? "N/A" : Info.routedTo.serviceName,
                                    Description = string.IsNullOrWhiteSpace(Info.description) ? "N/A" : Info.description,
                                    Geolocation = CountryCodeConverter(Info.country),
                                    EdgeRules = "N/A",
                                    FlagLink = $"https://cdn.ipregistry.co/flags/noto/{Info.country}.png",
                                    UnderAttack = false,

                                });
                            }
                            else
                            {
                                //var Info = JsonSerializer.Deserialize<IPInfo>(OVHCloud.GetAsync($"/ip/{newip}/30").Result);
                                IPAddressRange Range = IPAddressRange.Parse(IP);
                                foreach (var IPAddress in Range)
                                {
                                    database.IPs.Add(new Database.IPSchema
                                    {
                                        IP = IPAddress,
                                        Server = "N/A",
                                        Description = "N/A",
                                        Geolocation = CountryCodeConverter(null),
                                        EdgeRules = "N/A",
                                        FlagLink = $"https://cdn.ipregistry.co/flags/noto/N/A.png",
                                        UnderAttack = false,
                                    });
                                }
                            }
                            
                        }
                    }
                    database.SaveChanges();
                    return Task.CompletedTask;
                }
                else
                {
                    Console.WriteLine("Please login");
                    return Task.CompletedTask;
                }
            }
            catch(Exception e)
            {
                return Task.FromException(e);
            }
        }

        private static string CountryCodeConverter(string code)
        {
            return code switch
            {
                "uk" => "United Kingdom",
                "es" => "Spain",
                "pl" => "Poland",
                "nl" => "Netherlands",
                "lt" => "Lithuania",
                "it" => "Italy",
                "ie" => "Ireland",
                "fr" => "France",
                "cz" => "Czech Republic",
                "be" => "Belgium",
                "fi" => "Finland",
                "pt" => "Portugal",
                "de" => "Germany",
                "us" => "United States",
                "ca" => "Canada",
                _ => "N/A",
            };
        }
        private class IPMitigationStats
        {
            public string state { get; set; }
            public bool auto { get; set; }
            public string ipOnMitigation { get; set; }
            public bool permanent { get; set; }
        }

        public static void Detection()
        {
            Console.WriteLine("starting attack detection");
            new Thread(() =>
            {
                var database = new Services.DatabaseService();
                while (true)
                {
                    foreach (var IP in database.IPs)
                    {
                        try
                        {
                            string MitigationResult = OVHCloud.GetAsync($"/ip/{IP.IP}/mitigation").Result;
                            if (MitigationResult.Contains("[]"))
                            {
                                if (IP.UnderAttack)
                                {
                                    //execute notify
                                    AttackDetection(IP);
                                }
                            }
                            else
                            {
                                //checking permanent mitigation ips
                                IPMitigationStats MitigationInfo = null;
                                try
                                {
                                    MitigationInfo = JsonSerializer.Deserialize<IPMitigationStats>(OVHCloud.GetAsync($"/ip/{IP.IP}/mitigation/{IP.IP}").Result);
                                }
                                catch { }
                                if (MitigationInfo != null)
                                    switch (MitigationInfo.auto)
                                    {
                                        case true:
                                            if (!IP.UnderAttack)
                                            {
                                                //notify
                                                AttackDetected(IP);
                                            }
                                            break;
                                        default:
                                            //notify
                                            if (IP.UnderAttack)
                                                AttackDetection(IP);
                                            break;
                                    }
                            }
                        }
                        catch(Exception)
                        {
                        }
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }

            }).Start();
        }

        public static void AttackDetected(Database.IPSchema IP)
        {
            //log attack
            var database = new Services.DatabaseService();
            database.Attacks.Add(new Database.LogsSchema.AttackLogs()
            {
                Server = IP.Server,
                IPAttacked = IP.IP,
                DetectionTime = DateTime.Now,
                Duration = "N/A",
                EndingTime = DateTime.MinValue,
                Active = true
            });
            database.IPs.Find(IP).UnderAttack = true;
            database.SaveChanges();
            
            var client = new DiscordWebhookClient(database.Setttings.FirstOrDefault(x => x.ProfileName == Profile).DiscordWebHookUrl);
            var RichEmbed = new EmbedBuilder
            {
                Title = "Possible DDoS Attack Detected",
                Description = "OVH has detected a network alert.",
                Author = new EmbedAuthorBuilder().WithName($"Nebula Mods Inc. | OVH API Beta V{Assembly.GetExecutingAssembly().GetName().Version}").WithUrl("https://nebulamods.ca").WithIconUrl("https://nebulamods.ca/content/media/images/ovh-cloud.png"),
                Url = "https://nebulamods.ca",
                Footer = new EmbedFooterBuilder().WithText($"Detection Time: {DateTime.Now:h:mm:ss tt MMMM dd/yyyy}").WithIconUrl("https://nebulamods.ca/content/media/images/ddos-alert.png"),//need new picture
                ThumbnailUrl = IP.FlagLink,//dynamic flag injector
                Color = RandomDiscordColour(),
                Fields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder().WithName("Server").WithValue(IP.Server).WithIsInline(true),
                        new EmbedFieldBuilder().WithName("IP Address").WithValue($"{IP.IP.ToString().Split('.')[0]}.{IP.IP.ToString().Split('.')[1]}.{IP.IP.ToString().Split('.')[2]}.*").WithIsInline(true),
                    }
            }.Build();

            //send notify
            ulong messageID = 0;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (messageID == 0)
                        messageID = client.SendMessageAsync(embeds: new[] { RichEmbed }).Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        public static void AttackDetection(Database.IPSchema IP)
        {
            DateTime complete = DateTime.Now;
            var database = new Services.DatabaseService();
            Database.LogsSchema.AttackLogs Logs = null;
            foreach (var Attacks in database.Attacks)
            {
                if (Attacks.Active && Attacks.IPAttacked == IP.IP)
                    Logs = Attacks;
            }
            Logs.EndingTime = complete;
            Logs.Duration = $"{Math.Round((complete - Logs.DetectionTime).TotalMinutes, 2)} Minutes";
            Logs.Active = false;
            database.IPs.Find(IP).UnderAttack = false;
            database.SaveChanges();
            var client = new DiscordWebhookClient(database.Setttings.FirstOrDefault(x => x.ProfileName == Profile).DiscordWebHookUrl);
            var RichEmbed = new EmbedBuilder
            {
                Title = "DDoS Attack Complete",
                Description = "OVH has removed this IP from active mitigation.",
                Author = new EmbedAuthorBuilder().WithName($"Nebula Mods Inc. | OVH API Beta V{Assembly.GetExecutingAssembly().GetName().Version}").WithUrl("https://nebulamods.ca").WithIconUrl("https://nebulamods.ca/content/media/images/Home.png"),
                Url = "https://nebulamods.ca",
                Footer = new EmbedFooterBuilder().WithText($"No Longer Detected: {Logs.EndingTime:h:mm:ss tt MMMM dd/yyyy} ADT").WithIconUrl("https://nebulamods.ca/content/media/images/protection.png"),//need new picture
                ThumbnailUrl = database.IPs.FirstOrDefault(x => x.IP == Logs.IPAttacked).FlagLink,
                Color = RandomDiscordColour(),
                Fields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder().WithName("Server").WithValue(IP.Server).WithIsInline(true),
                        new EmbedFieldBuilder().WithName("IP Address").WithValue(IP.IP.ToString()).WithIsInline(true),
                        new EmbedFieldBuilder().WithName("Duration").WithValue(Logs.Duration).WithIsInline(true),

                    }
            }.Build();

            //send notify
            ulong messageID = 0;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (messageID == 0)
                        messageID = client.SendMessageAsync(embeds: new[] { RichEmbed }).Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        private static Color RandomDiscordColour()
        {
            return new Color(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));
        }
    }

    public static class StringExtensions
    {
        public static string MakeIPAddress(this string str)
        {
            return Regex.Replace(str, "[^0-9,.:/]+", "", RegexOptions.Compiled);
        }
    }

}
