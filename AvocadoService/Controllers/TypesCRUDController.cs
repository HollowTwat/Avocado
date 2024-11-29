using AvocadoDb.DbModels;
using AvocadoParser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public async Task<string> ParseSite()
        {
            ParserHelper parserHelper = new ParserHelper();
            var products = await parserHelper.ParceSite();
            var productsDisc = products.Distinct().ToList();
            await _context.Products.AddRangeAsync(productsDisc);
            await _context.SaveChangesAsync();
            return $"count={products.Count}, distinct={productsDisc.Count}";
        }
    }
}
