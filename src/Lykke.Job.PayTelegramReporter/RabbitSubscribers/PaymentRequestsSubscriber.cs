using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using System;
using System.Threading.Tasks;
using Lykke.Job.PayTelegramReporter.Domain.Services;
using Lykke.Job.PayTelegramReporter.Settings.JobSettings;
using Lykke.Service.PayInternal.Contract.PaymentRequest;

namespace Lykke.Job.PayTelegramReporter.RabbitSubscribers
{
    public class PaymentRequestsSubscriber : IStartable, IStopable
    {
        private readonly IPaymentRequestStatusReporter _paymentRequestStatusReporter;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;
        private readonly RabbitMqSettings _settings;
        private RabbitMqSubscriber<PaymentRequestDetailsMessage> _subscriber;

        public PaymentRequestsSubscriber(IPaymentRequestStatusReporter paymentRequestStatusReporter,
            ILogFactory logFactory, RabbitMqSettings settings)
        {
            _paymentRequestStatusReporter = paymentRequestStatusReporter;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
            _settings = settings;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString, _settings.ExchangeName, 
                    nameof(PayTelegramReporter));

            _subscriber = new RabbitMqSubscriber<PaymentRequestDetailsMessage>(
                    _logFactory,
                    settings,
                    new ResilientErrorHandlingStrategy(
                        _logFactory,
                        settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<PaymentRequestDetailsMessage>())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();

            _log.Info($"<< {nameof(PaymentRequestsSubscriber)} is started.");
        }

        private Task ProcessMessageAsync(PaymentRequestDetailsMessage message)
        {
            return _paymentRequestStatusReporter.ReportAsync(message);
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber?.Stop();
            _log.Info($"<< {nameof(PaymentRequestsSubscriber)} is stopped.");
        }
    }
}
