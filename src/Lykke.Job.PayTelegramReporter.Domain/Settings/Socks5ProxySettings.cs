using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.PayTelegramReporter.Domain.Settings
{
    public class Socks5ProxySettings
    {
        [Optional]
        public string Host { get; set; }

        [Optional]
        public int Port { get; set; }

        [Optional]
        public string Username { get; set; }

        [Optional]
        public string Password { get; set; }
    }
}
