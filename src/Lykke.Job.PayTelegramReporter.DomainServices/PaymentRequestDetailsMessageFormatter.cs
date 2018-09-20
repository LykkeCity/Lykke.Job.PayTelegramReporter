using System.Collections.Generic;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using System.Linq;
using Lykke.Job.PayTelegramReporter.Domain.Services;

namespace Lykke.Job.PayTelegramReporter.DomainServices
{
    public class PaymentRequestDetailsMessageFormatter: IPaymentRequestDetailsMessageFormatter
    {
        public string FormatPaymentCompleted(PaymentRequestDetailsMessage message)
        {
            return string.Format(MessageTemplates.PaymentCompleted, message.WalletAddress,
                GetSourceWalletAddressesText(message.Transactions), message.PaidAmount, 
                message.PaymentAssetId);
        }

        public string FormatRefundRequired(PaymentRequestDetailsMessage message)
        {
            return string.Format(MessageTemplates.RefundRequired, message.WalletAddress,
                GetSourceWalletAddressesText(message.Transactions), message.PaidAmount, 
                message.PaymentAssetId);
        }

        private string GetSourceWalletAddressesText(IEnumerable<PaymentRequestTransaction> transactions)
        {
            string[] sourceWallets = transactions?.SelectMany(t => t.SourceWalletAddresses).Distinct().ToArray();
            if (sourceWallets?.Any() == true)
            {
                return string.Join(", ", sourceWallets);
            }

            return MessageTemplates.Empty;
        }
    }
}
