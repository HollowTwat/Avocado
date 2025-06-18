using AvocadoService.AvocadoServiceDb.DbModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NutriDbService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AvocadoService.Helpers
{
    public class NotificationHelper
    {

        private readonly static string _api_key = "f3ccf95cf601c3fb7efe18c3b6135d4a";
        private string Htoken = "7557028885:AAHmFRXd5C2gtJafVg0XVYQjA_VNkbIB5k8";
        private readonly static string _reqUrl = $"https://chatter.salebot.pro/api/{_api_key}/message";
        private readonly static string _lessondonemess = "36169317";
        private readonly static string _lessonforgotmess = "36327038";
        private readonly static string _ndiarynmealmess = "36645266";
        private railwayContext _context;
        private readonly ILogger _logger;

        public NotificationHelper(railwayContext context, IServiceScopeFactory serviceProviderFactory)
        {
            _context = context;
            _logger = serviceProviderFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger<NotificationHelper>>();
        }
        private async Task SendNot(long ClientId, string MessBoxId)
        {
            var reqparams = new NocodeNot { client_id = ClientId, message_id = MessBoxId };
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(100);
            HttpContent content = new StringContent(JsonConvert.SerializeObject(reqparams), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_reqUrl, content);
            var r = await response.Content.ReadAsStringAsync();
        }
        public async Task SendVoteNotificationSingle(long UserTgId)
        {
            try
            {
                _logger.LogWarning($"User:{UserTgId} SendNotification");

                var botClient = new TelegramBotClient(Htoken);
                var mess = $"🌿 <i>Богини, как вы оцените свое последнее взаимодействие с Avocado Ботом?</i>\r\n\r\nОцените от 1 до 10, где 1 —  «очень плохо», а 10 — «отлично».";
                var buttons = new List<InlineKeyboardButton>();
                for (int i = 1; i <= 10; i++)
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData($"{i}", $"vote_{i}"));
                }
                List<InlineKeyboardButton> firstHalf = buttons.GetRange(0, 5);  
                List<InlineKeyboardButton> secondHalf = buttons.GetRange(5, 5); 
                await botClient.SendTextMessageAsync(
                    chatId: UserTgId,
                    text: mess,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    replyMarkup: new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>> { firstHalf, secondHalf })
                ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"NotificationSendError for User:{UserTgId}", ex);
                await ErrorHelper.SendErrorMess($"NotificationSendError for User:{UserTgId}", ex);
            }

        }
    }

    public class NocodeNot
    {
        public string message_id { get; set; }

        public long client_id { get; set; }
    }
}
