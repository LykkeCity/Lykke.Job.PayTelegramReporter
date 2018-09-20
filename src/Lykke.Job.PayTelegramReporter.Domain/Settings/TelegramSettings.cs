using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.PayTelegramReporter.Domain.Settings
{
    public class TelegramSettings
    {
        [Optional]
        public Socks5ProxySettings ProxySettings { get; set; }

        public string Token { get; set; }
    }
}
