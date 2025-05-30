﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using AvocadoService.AvocadoServiceDb.DbModels;
using AvocadoService.Models.PayModel;

namespace AvocadoService.Helpers
{

    public class SubscriptionHelper
    {
        private readonly static string _api_key = "a87df4c5cb5c6c8cc36ad24c8db5e54b";
        //private readonly static string _succmess = "38327266";
        public railwayContext _railwayContext { get; set; }
        private readonly IServiceScopeFactory _serviceProviderFactory;
        private readonly ILogger _logger;
        public SubscriptionHelper(railwayContext nutriDbContext, IServiceScopeFactory serviceProviderFactory)
        {
            _railwayContext = nutriDbContext;
            _serviceProviderFactory = serviceProviderFactory;
            _logger = _serviceProviderFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger<SubscriptionHelper>>();
        }

        public async Task<bool> CheckSub(long TgId)
        {
            var user = await _railwayContext.Users.SingleOrDefaultAsync(x => x.UserTgId == TgId);
            //return true;
            return user?.IsActive == true;
        }
        public SuccessPayRequest ConvertToPayRequestJSON(string input)
        {
            var json = ConvertRequestToJSON(input);
            return JsonConvert.DeserializeObject<SuccessPayRequest>(json);
        }
        public SuccessInfoPayRequest ConvertToInfoPayRequestJSON(string input)
        {
            var json = ConvertRequestToJSON(input);
            return JsonConvert.DeserializeObject<SuccessInfoPayRequest>(json);
        }
        public FailPayRequest ConvertToFailRequestJSON(string input)
        {
            var json = ConvertRequestToJSON(input);
            return JsonConvert.DeserializeObject<FailPayRequest>(json);
        }
        public RecurrentRequest ConvertToReqRequestJSON(string input)
        {
            var json = ConvertRequestToJSON(input);
            return JsonConvert.DeserializeObject<RecurrentRequest>(json);
        }
        
        public async Task<bool> SendEmailInfo(string Email, string planLabel)
        {
            try
            {
                int labelId;
                //return true;
                if (string.IsNullOrEmpty(Email))
                {
                    await ErrorHelper.SendErrorMess("Упали при отправке уведомления на Email, Пустой Email");
                    return false;
                }
                switch (planLabel.ToLower().Trim())
                {
                    case "подписка навсегда":
                        labelId = 0;
                        break;
                    case "подписка на 3 месяца":
                        labelId = 3;
                        break;
                    case "подписка на 1 год":
                        labelId = 12;
                        break;
                    case "золотой билет":
                        labelId = 13;
                        break;
                    default:
                        labelId = 99;
                        await ErrorHelper.SendErrorMess($"Упали при отправке уведомления на Email, неизвестный planLabel {planLabel}");
                        break;

                }
                //if(string.IsNullOrEmpty(planLabel)) 
                string url = $"https://myavocadobot.ru/?success_pay_email={Email}&plan_id={labelId}";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // выбросит исключение, если ответ неудачен
                await ErrorHelper.SendSystemMess($"Отправили уведомление на Email: {Email}");
                var res = response.IsSuccessStatusCode;
                return res;
            }
            //catch (HttpRequestException ex)
            catch (Exception ex)
            {
                await ErrorHelper.SendErrorMess($"Упали при отправке уведомления на Email:{Email}", ex);
                return false;
            }
        }

        private string ConvertRequestToJSON(string input)
        {
            var entries = input.Split('&');
            var transactionDict = new Dictionary<string, object>();

            foreach (var entry in entries)
            {
                var keyValue = entry.Split('=');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0];
                    string value = Uri.UnescapeDataString(keyValue[1]);

                    if (key == "Data" || key == "CustomFields")
                    {
                        transactionDict.Add(key, JsonConvert.DeserializeObject(value));
                    }
                    else
                    {
                        transactionDict.Add(key, value);
                    }
                }
            }

            // Serialize dictionary to JSON format
            return JsonConvert.SerializeObject(transactionDict, Formatting.Indented);

        }
        private async Task SendSuccNot(long ClientNoId, string MessBoxId)
        {
            var reqparams = new NocodeNot { client_id = ClientNoId, message_id = MessBoxId };
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(100);
            HttpContent content = new StringContent(JsonConvert.SerializeObject(reqparams), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://chatter.salebot.pro/api/{_api_key}/message", content);
            var r = await response.Content.ReadAsStringAsync();
        }
        private async Task<long> GetUserNoId(long userTgId)
        {
            var reqparams = new NocodeGet { platform_ids = new List<string> { $"{userTgId}" }, group_id = "nutri_pay_bot" };
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(100);
            HttpContent content = new StringContent(JsonConvert.SerializeObject(reqparams), Encoding.UTF8, "text/plain");
            var response = await client.PostAsync($"https://chatter.salebot.pro/api/{_api_key}/find_client_id_by_platform_id", content);
            var r = await response.Content.ReadAsStringAsync();
            JArray jsonArray = JArray.Parse(r);

            // Извлекаем первый объект из массива
            JObject jsonObject = (JObject)jsonArray[0];

            // Получаем значение свойства "id"
            long id = jsonObject.Value<long>("id");
            return id;
        }
        public class NocodeGet
        {
            public List<string> platform_ids { get; set; }

            public string group_id { get; set; }
        }
    }
}
