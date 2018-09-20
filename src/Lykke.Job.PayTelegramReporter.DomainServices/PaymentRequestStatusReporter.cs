using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.PayTelegramReporter.Domain.Services;
using Lykke.Job.PayTelegramReporter.Domain.Settings;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.PayTelegramReporter.DomainServices
{
    public class PaymentRequestStatusReporter : IPaymentRequestStatusReporter
    {
        private readonly IPaymentRequestDetailsMessageFormatter _formatter;
        private readonly ITelegramSender _telegramSender;
        private readonly PaymentRequestStatusReporterSettings _settings;
        private readonly ILog _log;

        public PaymentRequestStatusReporter(IPaymentRequestDetailsMessageFormatter formatter,
            ITelegramSender telegramSender, PaymentRequestStatusReporterSettings settings, 
            ILogFactory logFactory)
        {
            _formatter = formatter;
            _telegramSender = telegramSender;
            _settings = settings;
            _log = logFactory.CreateLog(this);
        }

        public Task ReportAsync(PaymentRequestDetailsMessage message)
        {
            switch (message.Status)
            {
                case PaymentRequestStatus.Confirmed:
                    return ReportConfirmedAsync(message);
            }

            return Task.CompletedTask;
        }

        private Task ReportConfirmedAsync(PaymentRequestDetailsMessage message)
        {
            if (!_settings.MerchantIds.Contains(message.MerchantId, StringComparer.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            if (message.Transactions?.Any(t =>
                    t.SourceWalletAddresses?.Contains(_settings.LykkeXHotWallet, StringComparer.OrdinalIgnoreCase) ==
                    true) == true)
            {
                return ReportPaymentCompletedAsync(message);
            }

            return ReportRefundRequiredAsync(message);
        }

        private Task ReportPaymentCompletedAsync(PaymentRequestDetailsMessage message)
        {
            string text = _formatter.FormatPaymentCompleted(message);
            _log.Info(text, GetLogContext(message));
            return _telegramSender.SendTextMessageAsync(_settings.ChatId, text);
        }

        private Task ReportRefundRequiredAsync(PaymentRequestDetailsMessage message)
        {
            string text = _formatter.FormatRefundRequired(message);
            _log.Info(text, GetLogContext(message));
            return _telegramSender.SendTextMessageAsync(_settings.ChatId, text);
        }

        private object GetLogContext(PaymentRequestDetailsMessage message)
        {
            return new
            {
                PaymentRequestId = message.Id,
                message.MerchantId
            };
        }
    }
}
