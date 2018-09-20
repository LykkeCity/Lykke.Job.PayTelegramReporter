using Autofac;
using Common;
using Lykke.Job.PayTelegramReporter.Domain.Services;
using Lykke.Job.PayTelegramReporter.DomainServices;
using Lykke.Job.PayTelegramReporter.RabbitSubscribers;
using Lykke.Job.PayTelegramReporter.Services;
using Lykke.Job.PayTelegramReporter.Settings.JobSettings;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.SettingsReader;

namespace Lykke.Job.PayTelegramReporter.Modules
{
    public class JobModule : Module
    {
        private readonly PayTelegramReporterJobSettings _settings;
        private readonly IReloadingManager<PayTelegramReporterJobSettings> _settingsManager;

        public JobModule(PayTelegramReporterJobSettings settings, IReloadingManager<PayTelegramReporterJobSettings> settingsManager)
        {
            _settings = settings;
            _settingsManager = settingsManager;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            RegisterRabbitMqSubscribers(builder);
            RegisterServices(builder);
        }

        private void RegisterRabbitMqSubscribers(ContainerBuilder builder)
        {
            builder.RegisterType<PaymentRequestsSubscriber>()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("settings", _settings.PaymentRequestsSubscriber);
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<TelegramService>()
                .As<ITelegramSender>()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("settings", _settings.Telegram);

            builder.RegisterType<PaymentRequestDetailsMessageFormatter>()
                .As<IPaymentRequestDetailsMessageFormatter>();

            builder.RegisterType<PaymentRequestStatusReporter>()
                .As<IPaymentRequestStatusReporter>()
                .SingleInstance()
                .WithParameter("settings", _settings.PaymentRequestStatusReporter);
        }
    }
}
