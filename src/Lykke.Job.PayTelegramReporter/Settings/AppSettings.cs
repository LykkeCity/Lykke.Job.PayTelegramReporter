using Lykke.Job.PayTelegramReporter.Settings.JobSettings;
using Lykke.Job.PayTelegramReporter.Settings.SlackNotifications;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.PayTelegramReporter.Settings
{
    public class AppSettings
    {
        public PayTelegramReporterJobSettings PayTelegramReporterJob { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Optional]
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}
