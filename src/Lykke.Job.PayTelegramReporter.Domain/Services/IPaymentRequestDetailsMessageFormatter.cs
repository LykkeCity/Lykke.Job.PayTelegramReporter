using Lykke.Service.PayInternal.Contract.PaymentRequest;

namespace Lykke.Job.PayTelegramReporter.Domain.Services
{
    public interface IPaymentRequestDetailsMessageFormatter
    {
        string FormatPaymentCompleted(PaymentRequestDetailsMessage message);
        string FormatRefundRequired(PaymentRequestDetailsMessage message);
    }
}
