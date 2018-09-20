using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.PayTelegramReporter.Domain.Services;
using Lykke.Job.PayTelegramReporter.Domain.Settings;
using MihaZupan;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lykke.Job.PayTelegramReporter.DomainServices
{
    public class TelegramService : IStartable, IStopable, ITelegramSender
    {
        private readonly TelegramSettings _settings;
        private readonly ILog _log;
        private TelegramBotClient _client;

        public TelegramService(TelegramSettings settings, ILogFactory logFactory)
        {
            _settings = settings;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(_settings.ProxySettings?.Host))
            {
                _client = new TelegramBotClient(_settings.Token);
            }
            else
            {
                var proxy = new HttpToSocks5Proxy(_settings.ProxySettings.Host, _settings.ProxySettings.Port,
                    _settings.ProxySettings.Username, _settings.ProxySettings.Password);
                _client = new TelegramBotClient(_settings.Token, proxy);
            }

            _client.SetWebhookAsync("").GetAwaiter().GetResult();
            _client.OnReceiveError += ClientOnOnReceiveError;
            _client.OnReceiveGeneralError += ClientOnOnReceiveGeneralError;

            _client.StartReceiving(Array.Empty<UpdateType>());
        }

        private void ClientOnOnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs receiveGeneralErrorEventArgs)
        {
            _log.Error(receiveGeneralErrorEventArgs.Exception);
        }

        private void ClientOnOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            _log.Error(receiveErrorEventArgs.ApiRequestException);
        }

        public async Task SendTextMessageAsync(ChatId chatId, string text, ParseMode parseMode = ParseMode.Default, 
            bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = 0, 
            IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _client.SendTextMessageAsync(chatId, text, parseMode, disableWebPagePreview,
                    disableNotification, replyToMessageId, replyMarkup, cancellationToken);
            }
            catch (ChatNotFoundException ex)
            {
                _log.Error(ex, $"Bot's username {chatId.ToJson()} is not found.");
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            _client.StopReceiving();
        }
    }
}
