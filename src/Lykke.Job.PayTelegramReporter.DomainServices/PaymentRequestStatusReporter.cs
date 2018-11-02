using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.PayTelegramReporter.Domain.Services;
using Lykke.Job.PayTelegramReporter.Domain.Settings;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

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
                    return ReportConfirmedAsync(message, false);
                case PaymentRequestStatus.Error:
                    switch (message.ProcessingError)
                    {
                        case PaymentRequestProcessingError.LatePaid:
                        case PaymentRequestProcessingError.PaymentAmountBelow:
                        case PaymentRequestProcessingError.PaymentAmountAbove:
                            return ReportConfirmedAsync(message, true);
                    }
                    break;
            }

            return Task.CompletedTask;
        }

        private Task ReportConfirmedAsync(PaymentRequestDetailsMessage message, bool isPaidWithError)
        {
            bool isKycRequired = _settings.MerchantIds.Contains(message.MerchantId, StringComparer.OrdinalIgnoreCase);

            if (isKycRequired)
            {
                if (!isPaidWithError &&
                    message.Transactions?.Any(t =>
                        t.SourceWalletAddresses?.Any(a =>
                            _settings.LykkeXHotWallets.Contains(a, StringComparer.OrdinalIgnoreCase)) ==
                        true) == true)
                {
                    return ReportPaymentCompletedAsync(message, isKycRequired);
                }
                
                return ReportRefundRequiredAsync(message, isKycRequired);
            }

            if (!isPaidWithError)
            {
                ReportPaymentCompletedAsync(message, isKycRequired);
            }

            return ReportRefundRequiredAsync(message, isKycRequired);
        }

        private Task ReportPaymentCompletedAsync(PaymentRequestDetailsMessage message, bool isKycRequired)
        {
            string text = _formatter.FormatPaymentCompleted(message, isKycRequired);
            _log.Info(text, GetLogContext(message));
            return _telegramSender.SendTextMessageAsync(_settings.ChatId, text, ParseMode.Markdown);
        }

        private Task ReportRefundRequiredAsync(PaymentRequestDetailsMessage message, bool isKycRequired)
        {
            string text = _formatter.FormatRefundRequired(message, isKycRequired);
            _log.Info(text, GetLogContext(message));
            return _telegramSender.SendTextMessageAsync(_settings.ChatId, text, ParseMode.Markdown);
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
