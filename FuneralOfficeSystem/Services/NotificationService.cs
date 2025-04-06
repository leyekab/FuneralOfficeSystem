using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FuneralOfficeSystem.Services
{
    public class NotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendTelegramNotification(string message)
        {
            try
            {
                var botToken = _configuration["Telegram:BotToken"];
                var chatId = _configuration["Telegram:ChatId"];

                if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(chatId))
                {
                    _logger.LogWarning("Telegram configuration is missing. Notification not sent.");
                    return;
                }

                var client = new HttpClient();
                var url = $"https://api.telegram.org/bot{botToken}/sendMessage";

                var content = new StringContent(
                    JsonConvert.SerializeObject(new { chat_id = chatId, text = message }),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Telegram notification sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Telegram notification");
            }
        }

        public async Task SendViberNotification(string message)
        {
            try
            {
                var authToken = _configuration["Viber:AuthToken"];
                var receiverId = _configuration["Viber:ReceiverId"];

                if (string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(receiverId))
                {
                    _logger.LogWarning("Viber configuration is missing. Notification not sent.");
                    return;
                }

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Viber-Auth-Token", authToken);

                var url = "https://chatapi.viber.com/pa/send_message";

                var content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        receiver = receiverId,
                        type = "text",
                        text = message
                    }),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Viber notification sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Viber notification");
            }
        }
    }
}