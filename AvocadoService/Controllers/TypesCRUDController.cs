using AvocadoService.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

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
        public string Test()
        {
            var res = _context.Products.Where(x => x.Name == "Test");
            return Newtonsoft.Json.JsonConvert.SerializeObject(res);
        }
    }
}
