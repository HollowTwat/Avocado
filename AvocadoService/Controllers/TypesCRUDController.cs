using AvocadoServiceDb.DbModels;
using AvocadoServiceParser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvocadoService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TypesCRUDController : Controller
    {
        private readonly ILogger<TypesCRUDController> _logger;
        private railwayContext _context;

        public TypesCRUDController(railwayContext context, ILogger<TypesCRUDController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        public async Task<string> ParseSite(int listFrom, int listTo, string Cat)
        {
            try
            {
                ParserHelper parserHelper = new ParserHelper();
                var productsUrls = await parserHelper.GetProductUrlsList(listFrom, listTo, Cat);
                //var productsUrlsDis = productsUrls.Distinct().ToList();
                var existurl = _context.Products.Where(x => productsUrls.Contains(x.Url)).Select(x => x.Url).ToList();
                var productsUrlsDis = productsUrls.Except(existurl).ToList();
                var ciclec = productsUrlsDis.Count() / 100;
                for (var i = 0; i <= ciclec; i++)
                {
                    Console.WriteLine($"i={i}");
                    List<string> urls;
                    if (i != ciclec)
                        urls = productsUrlsDis.GetRange(i * 100, 100);
                    else
                        urls = productsUrlsDis.GetRange(i * 100, productsUrlsDis.Count() - ciclec * 100);
                    var products = await parserHelper.ParceElements(urls);
                    var productsDisc = products.Distinct().ToList();
                    productsDisc.RemoveAll(x => x.Name == null);
                    //var exist = _context.Products.Where(x => productsDisc.Select(x => x.Name).ToList().Contains(x.Name)).Select(x => x.Name).ToList();
                    //productsDisc = productsDisc.Except(productsDisc.Where(x => exist.Contains(x.Name))).ToList();
                    await _context.Products.AddRangeAsync(productsDisc);
                    await _context.SaveChangesAsync();
                }

                return $"OK";
            }
            catch (Exception ex)
            {
                return "Fail";
            }
        }

        [HttpGet]
        public async Task<bool> ParseFile()
        {
            try
            {
                var text = await System.IO.File.ReadAllTextAsync(@"C:\\ReposMy\goldapple_uhod.json");
                var res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Product>>(text);
                var resd = res.Distinct().ToList();
                var excl = _context.Products.Where(x => resd.Select(x => x.Url).ToList().Contains(x.Url)).Select(x => x.Url).ToList();
                var resex = resd.Except(res.Where(x => excl.Contains(x.Url))).ToList();
                await _context.Products.AddRangeAsync(resex);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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
                var elem = _context.Products.SingleOrDefault(x => x.Id == Id);
                return Newtonsoft.Json.JsonConvert.SerializeObject(elem);

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
