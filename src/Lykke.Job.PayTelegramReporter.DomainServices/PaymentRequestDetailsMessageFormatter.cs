using System.Collections.Generic;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using System.Linq;
using Lykke.Job.PayTelegramReporter.Domain.Services;

namespace Lykke.Job.PayTelegramReporter.DomainServices
{
    public class PaymentRequestDetailsMessageFormatter: IPaymentRequestDetailsMessageFormatter
    {
        public string FormatPaymentCompleted(PaymentRequestDetailsMessage message, bool isKycRequired)
        {
            return string.Format(
                MessageTemplates.PaymentCompleted,
                message.MerchantId,
                isKycRequired ? MessageTemplates.KycRequiredText : string.Empty,
                message.WalletAddress,
                GetSourceWalletAddressesText(message.Transactions),
                message.PaidAmount,
                message.PaymentAssetId,
                message.Id
            );
        }

        public string FormatRefundRequired(PaymentRequestDetailsMessage message, bool isKycRequired)
        {
            return string.Format(
                MessageTemplates.RefundRequired,
                message.MerchantId,
                string.Empty,
                message.WalletAddress,
                GetSourceWalletAddressesText(message.Transactions),
                message.PaidAmount,
                message.PaymentAssetId,
                message.Order?.PaymentAmount,
                message.Id
            );
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
