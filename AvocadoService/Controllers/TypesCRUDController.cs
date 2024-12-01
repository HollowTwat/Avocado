using AvocadoDb.DbModels;
using AvocadoParser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
        public async Task<string> ParseSite(int multi, string Cat)
        {
            ParserHelper parserHelper = new ParserHelper();
            var productsUrls = await parserHelper.GetProductUrlsList(multi, Cat);
            var productsUrlsDis = productsUrls.Distinct().ToList();
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
                await _context.Products.AddRangeAsync(productsDisc);
                await _context.SaveChangesAsync();
            }

            return $"OK";
        }
    }
}
