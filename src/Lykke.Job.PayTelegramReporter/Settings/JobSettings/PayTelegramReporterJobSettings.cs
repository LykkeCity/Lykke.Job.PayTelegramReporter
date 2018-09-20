using Lykke.Job.PayTelegramReporter.Domain.Settings;

namespace Lykke.Job.PayTelegramReporter.Settings.JobSettings
{
    public class PayTelegramReporterJobSettings
    {
        public DbSettings Db { get; set; }
        public RabbitMqSettings PaymentRequestsSubscriber { get; set; }
        public TelegramSettings Telegram { get; set; }
        public PaymentRequestStatusReporterSettings PaymentRequestStatusReporter { get; set; }
    }
}
