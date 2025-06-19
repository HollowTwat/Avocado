using AngleSharp.Common;
using AvocadoService.AvocadoServiceDb.DbModels;
using AvocadoService.AvocadoServiceParser;
using AvocadoService.Helpers;
using AvocadoService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NutriDbService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AvocadoService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TypesCRUDController : Controller
    {
        private readonly ILogger<TypesCRUDController> _logger;
        private railwayContext _context;

        private readonly NotificationHelper _notificationHelper;
        public TypesCRUDController(railwayContext context, NotificationHelper notificationHelper, ILogger<TypesCRUDController> logger)
        {
            _context = context;
            _logger = logger;
            _notificationHelper = notificationHelper;
        }

        [HttpGet]
        public async Task<bool> GetDupl()
        {
            try
            {
                //         var newList = _context.Products.Select(o => new
                //         {
                //             Identifier = o.Id,
                //             FullName = o.Name
                //         })
                //.ToList();
                //         System.IO.File.WriteAllText(@"C:\\ReposMy\products.json", Newtonsoft.Json.JsonConvert.SerializeObject(newList));



                var dub = _context.Products.AsEnumerable()
                      .GroupBy(x => x.Url)
                     .Where(g => g.Count() > 1)
                     .SelectMany(g => g)
                     .ToList();
                var d2 = dub.DistinctBy(x => x.Url).ToList();
                _context.Products.RemoveRange(d2);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        public async Task<string> GetElementInfo(int Id)
        {
            try
            {
                var elem = await _context.Products.SingleOrDefaultAsync(x => x.Id == Id);
                return Newtonsoft.Json.JsonConvert.SerializeObject(elem);

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public async Task<bool> SetElementRate(GPTRateSaveRequest req)
        {
            try
            {
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> SetUserBaseData(SetUserBaseDataRequest req)
        {
            try
            {
                if (!long.TryParse(req.tg_id, out long user_tg_id))
                    return false;
                if (!int.TryParse(req.user_data.age, out int age))
                    age = 0;
                var users = _context.Users.Where(x => x.UserTgId == user_tg_id);
                if (users.Any())
                {

                    var user = await users.SingleAsync();
                    user.Lifestyle = req.user_data.lifestyle;
                    user.Allergy = req.user_data.allergy;
                    user.Age = age;
                    user.Activity = req.user_data.activity;
                    user.WaterIntake = req.user_data.water_intake;
                    user.Gender = req.user_data.gender;
                    user.Habits = req.user_data.habits;
                    user.Location = req.user_data.location;
                    user.Stress = req.user_data.stress;
                    _context.Users.Update(user);
                }
                else
                {
                    await _context.Users.AddAsync(new AvocadoServiceDb.DbModels.User
                    {
                        UserTgId = user_tg_id,
                        Stress = req.user_data.stress,
                        Activity = req.user_data.activity,
                        Age = age,
                        Allergy = req.user_data.allergy,
                        Gender = req.user_data.gender,
                        Location = req.user_data.location,
                        Habits = req.user_data.habits,
                        Lifestyle = req.user_data.lifestyle,
                        WaterIntake = req.user_data.water_intake,

                    });
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }

        }

        [HttpPost]
        public async Task<bool> SetUserHairData(SetUserHairDataRequest req)
        {
            try
            {
                if (!long.TryParse(req.tg_id, out long user_tg_id))
                    return false;
                var users = _context.Users.Where(x => x.UserTgId == user_tg_id);
                if (users.Any())
                {

                    var user = await users.SingleAsync();
                    user.Hairscalptype = req.user_hair_data.hair_scalp_type;
                    user.Hairthickness = req.user_hair_data.hair_thickness;
                    user.Hairlength = req.user_hair_data.hair_length;
                    user.Hairstructure = req.user_hair_data.hair_structure;
                    user.Haircondition = req.user_hair_data.hair_condition;
                    user.Hairgoals = req.user_hair_data.hair_goals;
                    user.Hairwashingfrequency = req.user_hair_data.washing_frequency;
                    user.Haircurrentproducts = req.user_hair_data.current_products;
                    user.Hairproducttexture = req.user_hair_data.product_texture;
                    user.Hairsensitivity = req.user_hair_data.sensitivity;

                    _context.Users.Update(user);
                }
                else
                {
                    await _context.Users.AddAsync(new AvocadoServiceDb.DbModels.User
                    {
                        UserTgId = user_tg_id,
                        Hairscalptype = req.user_hair_data.hair_scalp_type,
                        Hairthickness = req.user_hair_data.hair_thickness,
                        Hairlength = req.user_hair_data.hair_length,
                        Hairstructure = req.user_hair_data.hair_structure,
                        Haircondition = req.user_hair_data.hair_condition,
                        Hairgoals = req.user_hair_data.hair_goals,
                        Hairwashingfrequency = req.user_hair_data.washing_frequency,
                        Haircurrentproducts = req.user_hair_data.current_products,
                        Hairproducttexture = req.user_hair_data.product_texture,
                        Hairsensitivity = req.user_hair_data.sensitivity

                    });
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }

        }

        [HttpPost]
        public async Task<bool> SetUserBodyData(SetUserBodyDataRequest req)
        {
            try
            {
                if (!long.TryParse(req.tg_id, out long user_tg_id))
                    return false;
                var users = _context.Users.Where(x => x.UserTgId == user_tg_id);
                if (users.Any())
                {

                    var user = await users.SingleAsync();
                    user.Bodyskintype = req.user_body_data.body_skin_type;
                    user.Bodyskinsensitivity = req.user_body_data.body_skin_sensitivity;
                    user.Bodyskincondition = req.user_body_data.body_skin_condition;
                    user.Bodyhairissues = req.user_body_data.body_hair_issues;
                    user.Bodygoals = req.user_body_data.body_goals;

                    _context.Users.Update(user);
                }
                else
                {
                    await _context.Users.AddAsync(new AvocadoServiceDb.DbModels.User
                    {
                        UserTgId = user_tg_id,
                        Bodyskintype = req.user_body_data.body_skin_type,
                        Bodyskinsensitivity = req.user_body_data.body_skin_sensitivity,
                        Bodyskincondition = req.user_body_data.body_skin_condition,
                        Bodyhairissues = req.user_body_data.body_hair_issues,
                        Bodygoals = req.user_body_data.body_goals

                    });
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }

        }

        [HttpPost]
        public async Task<bool> SetUserFaceData(SetUserFaceDataRequest req)
        {
            try
            {
                if (!long.TryParse(req.tg_id, out long user_tg_id))
                    return false;
                var users = _context.Users.Where(x => x.UserTgId == user_tg_id);
                if (users.Any())
                {

                    var user = await users.SingleAsync();
                    user.Faceskintype = req.user_face_data.face_skin_type;
                    user.Faceskincondition = req.user_face_data.face_skin_condition;
                    user.Faceskinissues = req.user_face_data.face_skin_issues;
                    user.Faceskingoals = req.user_face_data.face_skin_goals;

                    _context.Users.Update(user);
                }
                else
                {
                    await _context.Users.AddAsync(new AvocadoServiceDb.DbModels.User
                    {
                        UserTgId = user_tg_id,
                        Faceskintype = req.user_face_data.face_skin_type,
                        Faceskincondition = req.user_face_data.face_skin_condition,
                        Faceskinissues = req.user_face_data.face_skin_issues,
                        Faceskingoals = req.user_face_data.face_skin_goals

                    });
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }

        }

        [HttpGet]
        public async Task<AvocadoServiceDb.DbModels.User> GetUserData(long userTgId)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.UserTgId == userTgId);
        }

        [HttpGet]
        public async Task<bool> StartVoteNotify()
        {
            var usersTgIds = await _context.Users.Where(x => x.IsActive == true).Select(x => x.UserTgId).ToListAsync();

            _ = Task.Run(async () =>
            {
                List<long> doneids = new List<long>();
                try
                {
                    await _notificationHelper.SendAlertToMe("Start Vote");
                    foreach (var userTgId in usersTgIds)
                    {
                        try
                        {
                            await _notificationHelper.SendVoteNotificationSingle(userTgId);
                            doneids.Add(userTgId);
                        }
                        catch (Exception ex)
                        {
                            await _notificationHelper.SendAlertToMe($"Exception on Vote id= {userTgId}");
                        }
                    }
                    await _notificationHelper.SendAlertToMe($"End Vote. Total={doneids.Count}");
                }
                catch (Exception ex)
                {
                    await _notificationHelper.SendAlertToMe("Exception on Vote");
                    _logger.LogError(ex, $"Exception on Vote:");
                    _logger.LogInformation($"DoneList={Newtonsoft.Json.JsonConvert.SerializeObject(doneids)}");
                    await _notificationHelper.SendAlertToMe("Exception on Vote:");
                }
            });

            return true;
        }

        [HttpGet]
        public async Task<bool> SetUserVote(long userTgId, short vote)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserTgId == userTgId);
                if (user == null)
                    return false;
                user.Vote = vote;
                _context.Users.Update(user);
                await _context.SaveChangesAsync(true);
                return true;
            }
            catch (Exception ex) { return false; }
        }
        //[HttpGet]
        //public async Task<bool> CheckUserSub(long userTgId)
        //{
        //    return _context.Users.SingleOrDefault(x => x.UserTgId == userTgId) != null;
        //}

    }
}