using Lykke.Service.PayInternal.Contract.PaymentRequest;
using System.Threading.Tasks;

namespace Lykke.Job.PayTelegramReporter.Domain.Services
{
    public interface IPaymentRequestStatusReporter
    {
        Task ReportAsync(PaymentRequestDetailsMessage message);
    }
}
