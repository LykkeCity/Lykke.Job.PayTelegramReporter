namespace Lykke.Job.PayTelegramReporter.Domain.Settings
{
    public class PaymentRequestStatusReporterSettings: TelegramReporterSettings
    {
        public string[] MerchantIds { get; set; }

        public string[] LykkeXHotWallets { get; set; }
    }
}
