using System.ComponentModel.DataAnnotations;

namespace OVHAPI.Database
{
    public class Settings
    {
        [Key]
        public string ProfileName { get; set; }

        public string ApplicationKey { get; set; }

        public string ApplicationSecret { get; set; }

        public string ConsumerKey { get; set; }

        public string EndPoint { get; set; }

        public string DiscordWebHookUrl { get; set; }
    }
}
