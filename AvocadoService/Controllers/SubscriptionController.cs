using AvocadoService.AvocadoServiceDb.DbModels;
using AvocadoService.Helpers;
using AvocadoService.Models.PayModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AvocadoService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SubscriptionController : Controller
    {
        private readonly ILogger<SubscriptionController> _logger;
        private railwayContext _context;
        private SubscriptionHelper _subscriptionHelper;
        private static readonly Guid secr = new Guid("35e24644-2dcb-4210-96a3-475ee63936f9");

        public SubscriptionController(railwayContext context, ILogger<SubscriptionController> logger, SubscriptionHelper subscriptionHelper)
        {
            _context = context;
            _logger = logger;
            _subscriptionHelper = subscriptionHelper;
        }

        //[HttpGet]
        //public async Task<bool> sss()
        //{
        //    List<long> ptgids = new List<long>();
        //    var subs = await _context.Subscriptions.Where(x => x.Type == "unknown").ToListAsync();
        //    foreach (var sub in subs)
        //    {
        //        try
        //        {
        //            string label = (string)JObject.Parse(sub.Extra)["Data"]["CloudPayments"]["CustomerReceipt"]["Items"][0]["label"];
        //            sub.Type = label;
        //        }
        //        catch {

        //            ptgids.Add(sub.UserTgId);
        //        }
        //    }
        //    var d = subs.Select(x => x.Type);
        //    _context.UpdateRange(subs);
        //await    _context.SaveChangesAsync();
        //    return true;
        //}

        [HttpGet]
        public async Task<string> Test()
        {
            try
            {
                await ErrorHelper.SendSystemMess("Test");
                return "OK";
            }
            catch (Exception ex) { return Newtonsoft.Json.JsonConvert.SerializeObject(ex); }
        }
        [HttpPost]
        public async Task<SubResponse> SuccessPay()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    string bodyContent = await reader.ReadToEndAsync();
                    var ress2 = HttpUtility.UrlDecode(bodyContent);
                    _logger.LogWarning(ress2);
                    SuccessPayRequest cl = _subscriptionHelper.ConvertToPayRequestJSON(ress2);
                    await ErrorHelper.SendSystemMess($"Пришел платеж без пользователя {Newtonsoft.Json.JsonConvert.SerializeObject(cl)}");
                    _logger.LogWarning(Newtonsoft.Json.JsonConvert.SerializeObject($"cl.Data={cl.Data.Values.First()}"));
                    // Парсим JSON в объект JObject
                    JObject root = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(cl.Data.Values.First()));

                    // Безопасно достаем поле "label" с помощью SelectToken
                    string planLabel = (string)root.SelectToken("CustomerReceipt.Items[0].label");
                    _logger.LogWarning($"Label={planLabel}");
                    await _context.Subscriptions.AddAsync(new Subscription
                    {
                        TransactionId = cl.TransactionId,
                        Amount = cl.Amount,
                        DateTime = cl.DateTime,
                        Status = cl.Status,
                        InvoiceId = cl.InvoiceId,
                        AccountId = cl.AccountId,
                        SubscriptionId = cl.SubscriptionId,
                        Email = cl.Email.ToLower().Trim(),
                        Rrn = cl.Rrn,
                        Type = planLabel,
                        //UserTgId = inputUserId,
                        IsActive = true,
                        IsLinked = false,
                        DateCreate = DateTime.UtcNow.ToLocalTime().AddHours(3),
                        DateUpdate = DateTime.UtcNow.ToLocalTime().AddHours(3),
                        Extra = Newtonsoft.Json.JsonConvert.SerializeObject(cl)
                    });

                    await _context.SaveChangesAsync();
                    _subscriptionHelper.SendEmailInfo(cl.Email, planLabel);

                }

                return new SubResponse { code = 0 };
            }
            catch (Exception ex)
            {
                await ErrorHelper.SendErrorMess("Упали при оформлении подписки", ex);
                return new SubResponse { code = 500 };
            }
        }

        [HttpPost]
        public async Task<bool> SendEmailManual(string email, int labelId)
        {
            try
            {
                string labelString;
                switch (labelId)
                {
                    case 6:
                        labelString = "подписка на 3 месяца";
                        break;
                    case 12:
                        labelString = "подписка на 1 год";
                        break;
                    case 0:
                    default:
                        labelString = "подписка навсегда";
                        break;
                }
                var res = await _subscriptionHelper.SendEmailInfo(email, labelString);
                if (!res)
                    await ErrorHelper.SendSystemMess($"Не смогли отправить пользователю {email} ссылку на бота после оплаты");
                return res;
            }
            catch (Exception ex)
            {
                await ErrorHelper.SendErrorMess($"Упали при отправке уведомления^ {email}", ex);
                return false;
            }
        }
        [HttpPost]
        public async Task<SubResponse> FailPay()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                string bodyContent = await reader.ReadToEndAsync();
                var ress2 = HttpUtility.UrlDecode(bodyContent);
                _logger.LogWarning(ress2);
                FailPayRequest cl = _subscriptionHelper.ConvertToFailRequestJSON(ress2);
                await ErrorHelper.SendSystemMess($"Fail:{Newtonsoft.Json.JsonConvert.SerializeObject(cl)}");
            }

            return new SubResponse { code = 0 };
        }

        [HttpPost]
        public async Task<SubResponse> CancelPay()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                string bodyContent = await reader.ReadToEndAsync();
                var ress2 = HttpUtility.UrlDecode(bodyContent);
                _logger.LogWarning(ress2);
                await ErrorHelper.SendSystemMess($"Cancel: {ress2}");
            }

            return new SubResponse { code = 0 };
        }

        [HttpPost]
        public async Task<SubResponse> RecurrentPay()
        {

            using (var reader = new StreamReader(Request.Body))
            {
                string bodyContent = await reader.ReadToEndAsync();
                var ress2 = HttpUtility.UrlDecode(bodyContent);
                _logger.LogWarning(ress2);
                RecurrentRequest cl = _subscriptionHelper.ConvertToReqRequestJSON(ress2);
                await ErrorHelper.SendSystemMess($"Recurent:{Newtonsoft.Json.JsonConvert.SerializeObject(cl)}");
                var sub = _context.Subscriptions.Where(x => x.SubscriptionId == cl.Id);
                if (sub?.Count() == 1)
                    await ErrorHelper.SendSystemMess($"Отменить подписку пользователя:{sub.First().UserTgId}");
                if (sub?.Count() > 1)
                    await ErrorHelper.SendSystemMess($"Больше 1 подписки пользователя:{sub.First().UserTgId}");
                if (sub?.Count() == 0)
                    await ErrorHelper.SendSystemMess($"Отмена несуществующей подписки : {cl.Id}");

            }
            return new SubResponse { code = 0 };
        }

        [HttpPost]
        public async Task<SubResponse> ConfirmPay()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                string bodyContent = await reader.ReadToEndAsync();
                var ress2 = HttpUtility.UrlDecode(bodyContent);
                _logger.LogWarning(ress2);
                await ErrorHelper.SendSystemMess($"Confirm: {ress2}");

            }
            return new SubResponse { code = 0 };
        }

        [HttpPost]
        public async Task<SubResponse> RefundPay()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                string bodyContent = await reader.ReadToEndAsync();
                var ress2 = HttpUtility.UrlDecode(bodyContent);
                _logger.LogWarning(ress2);
                await ErrorHelper.SendSystemMess($"Refund: {ress2}");

            }

            return new SubResponse { code = 0 };
        }

        [HttpPost]
        public async Task<bool> ActivateUser(long userTgId, string userEmail)
        {
            try
            {
                userEmail = userEmail.ToLower().Trim();
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserTgId == userTgId);
                if (user == null)
                    return false;
                user.Email = userEmail;
                if (user.IsActive)
                    return true;
                var readySub = await _context.Subscriptions.SingleOrDefaultAsync(x => x.IsLinked == false && x.IsActive == true && userEmail == x.Email.ToLower().Trim());
                if (readySub == null)
                    return false;



                readySub.IsLinked = true;
                readySub.UserTgId = userTgId;
                user.IsActive = true;

                _context.Users.Update(user);
                _context.Subscriptions.Update(readySub);
                await _context.SaveChangesAsync();
                await ErrorHelper.SendSystemMess($"Активирован пользователь ID={user.UserTgId}, Email={user.Email}");
                return true;
            }
            catch (Exception ex)
            {
                await ErrorHelper.SendErrorMess($"Ошибка активации пользователя {userTgId} Email:{userEmail}", ex);
                return false;
            }
        }

        [HttpGet]
        public async Task<List<Subscription>> GetSubs(string MyDateTime)
        {
            try
            {
                CheckSecret(HttpContext.Request);
                return await _context.Subscriptions.Where(x => x.DateCreate > DateTime.Parse(MyDateTime)).ToListAsync();
            }
            catch (Exception ex) { _logger.LogError(ex, "GetSubs"); throw; }
        }

        [HttpGet]
        public async Task<bool> AddSub(string Email)
        {
            Email = Email.ToLower().Trim();
            try
            {
                CheckSecret(HttpContext.Request);
                _context.Database.ExecuteSqlRaw("CALL \"public\".\"AddAccessToUserByEmail\"({0})", Email);
                await ErrorHelper.SendSystemMess($"Добавлена подписка на Email: {Email}");
                return true;
            }
            catch (Exception ex) { _logger.LogError(ex, "AddSub"); return false; }
        }

        [HttpGet]
        public async Task<List<Subscription>> GetUserSub(string Email)
        {
            Email = Email.ToLower().Trim();
            try
            {
                CheckSecret(HttpContext.Request);
                return await _context.Subscriptions.Where(x => x.Email == Email).ToListAsync();
            }
            catch (Exception ex) { _logger.LogError(ex, "GetUserSub"); throw; }
        }

        [HttpGet]
        public async Task<Subscription> GetUserSubDetail(long tgId)
        {

            try
            {
                //CheckSecret(HttpContext.Request);
                return await _context.Subscriptions.SingleOrDefaultAsync(x => x.UserTgId == tgId && x.IsLinked == true && x.IsActive == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserSub");
                await ErrorHelper.SendErrorMess($"Упали при получении деталей подписки", ex);

                throw;
            }
        }

        [HttpGet]
        public async Task<bool> DeactivateUser(string Email)
        {
            try
            {
                Email = Email.ToLower().Trim();
                CheckSecret(HttpContext.Request);
                var subs = await _context.Subscriptions.Where(x => x.Email.ToLower().Trim() == Email).ToListAsync();
                if (!subs.Any())
                    return false;
                var TgId = subs.Where(x => x.UserTgId != 0).FirstOrDefault()?.UserTgId;

                subs.ForEach(sub => sub.IsActive = false);
                AvocadoServiceDb.DbModels.User user = await _context.Users.SingleOrDefaultAsync(x => x.UserTgId == TgId);
                if (TgId != null)
                    user.IsActive = false;

                await _context.Database.BeginTransactionAsync();
                _context.Subscriptions.UpdateRange(subs);
                if (TgId != null)
                    _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                await ErrorHelper.SendSystemMess($"Отобрана подписка у Email: {Email}");
                return true;
            }
            catch (Exception ex) { _logger.LogError(ex, "DeactivateUser"); return false; }
        }

        private void CheckSecret(HttpRequest req)
        {
#if !DEBUG
                        if (!req.Headers.TryGetValue("MyTok", out var secP))
                            throw new AccessViolationException();
                        if (!Guid.TryParse(secP, out Guid headerGuid))
                            throw new AccessViolationException();
                        if (headerGuid != secr)
                            throw new AccessViolationException();
#endif
        }
    }

}
