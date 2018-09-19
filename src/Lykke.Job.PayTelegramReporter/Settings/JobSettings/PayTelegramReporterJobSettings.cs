namespace Lykke.Job.PayTelegramReporter.Settings.JobSettings
{
    public class PayTelegramReporterJobSettings
    {
        public DbSettings Db { get; set; }
        public RabbitMqSettings Rabbit { get; set; }
    }
}
