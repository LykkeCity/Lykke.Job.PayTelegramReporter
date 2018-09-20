using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.PayTelegramReporter.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
