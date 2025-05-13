using AvocadoService.AvocadoServiceDb.DbModels;
using AvocadoService.Helpers;
using AvocadoService.AvocadoServiceParser;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AvocadoTest
{
    [TestFixture]
    public class UnitTest1
    {
        public UnitTest1()
        {
            Setup();
        }
        private Mock<railwayContext> _mockContext;
        private Mock<IServiceScopeFactory> _mockServiceScopeFactory;
        private Mock<IServiceScope> _mockScope;
        private Mock<IServiceProvider> _mockServiceProvider;
        private Mock<ILogger<NotificationHelper>> _mockLogger;
        private railwayContext _railwayContext;
        [SetUp]
        public void Setup()
        {
            _railwayContext = new railwayContext();
            _mockContext = new Mock<railwayContext>();

            _mockLogger = new Mock<ILogger<NotificationHelper>>();
            // Создайте мок для ServiceProvider
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockServiceProvider.Setup(sp => sp.GetService(typeof(ILogger<NotificationHelper>)))
                                .Returns(_mockLogger.Object);
            // Создайте мок для IServiceScope
            _mockScope = new Mock<IServiceScope>();
            _mockScope.Setup(s => s.ServiceProvider).Returns(_mockServiceProvider.Object);
            // Создайте мок для IServiceScopeFactory и настройте возвращаемый scope
            _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            _mockServiceScopeFactory.Setup(s => s.CreateScope()).Returns(_mockScope.Object);
        }
        [Fact]
        public async Task TableToHeadersTest()
        {
            try
            {
                var table = _railwayContext.Products.Select(x => new { Identifier = x.Id, FullName = x.Name })
    .ToList();
                File.WriteAllText("output.json", JsonSerializer.Serialize(table));

                Xunit.Assert.True(true);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceNANOORGTest()
        {
            try
            {
                var source = "NANOORG0";
                var path = @"C:\\ReposMy\nanoorganic_products.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceKPCOSMTest()
        {
            try
            {
                var source = "KPCOSM";
                var path = @"C:\\ReposMy\kpcosm_list.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }

        //[Fact]
        public async Task ParceSOMELOVETest()
        {
            try
            {
                var source = "SOMELOVE";
                var path = @"C:\\ReposMy\somelove_products_list.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceANGELLABTest()
        {
            try
            {
                var source = "ANGELLAB";
                var path = @"C:\\ReposMy\angellab.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceJURASSTest()
        {
            try
            {
                var source = "JURASS";
                var path = @"C:\\ReposMy\jurassicspa_products_list.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceMIKOTest()
        {
            try
            {
                var source = "MIKO";
                var path = @"C:\\ReposMy\mi_ko.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceTINTBERRYTest()
        {
            try
            {
                var source = "TINTBERRY";
                var path = @"C:\\ReposMy\tintberry.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceLIVEORGTest()
        {
            try
            {
                var source = "LIVEORG";
                var path = @"C:\\ReposMy\productz.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }

        //[Fact]
        public async Task ParceECOMAKETest()
        {
            try
            {
                var source = "ECOMAKE";
                var path = @"C:\\ReposMy\ecomake.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceMAOKATest()
        {
            try
            {
                var source = "MAOKA";
                var path = @"C:\\ReposMy\maoka.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceMOREDOMATest()
        {
            try
            {
                var source = "MOREDOMA";
                var path = @"C:\\ReposMy\more_doma.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }

        //[Fact]
        public async Task ParceBIODEPOTest()
        {
            try
            {
                var source = "BIODEPO";
                var path = @"C:\\ReposMy\biodepo.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceYODOMETICSTest()
        {
            try
            {
                var source = "YODOMETICS";
                var path = @"C:\\ReposMy\yodometics.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceSIBIRBOTANICTest()
        {
            try
            {
                var source = "SIBIRBOTANICT";
                var path = @"C:\\ReposMy\sibirbotaniq.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceFEELNATURETest()
        {
            try
            {
                var source = "FEELNATURE";
                var path = @"C:\\ReposMy\feelnature.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }   
        //[Fact]
        public async Task ParceVIMTest()
        {
            try
            {
                var source = "VIM";
                var path = @"C:\\ReposMy\vim.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        private async Task<bool> ParceFileTest(string source, string path)
        {
            try
            {
                var text = await System.IO.File.ReadAllTextAsync(path);
                var res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PythParserElement>>(text);
                var resd = res.Distinct().ToList();
                var productList = new List<Product>();
                resd.ForEach(x =>
                {
                    var wp = decimal.TryParse(x.Weight, out decimal dWeight);
                    var vr = x?.Volume?.Trim()?.Replace(" ", "")?.Replace("мл", string.Empty)?.Replace("л", "000");
                    var vp = decimal.TryParse(vr, out decimal dVolume);

                    productList.Add(new Product
                    {
                        Brand = x.Brand,
                        Brandinfo = x.Brandinfo,
                        Consist = x.Consist,
                        Country = x.Country,
                        Description = x.Description,
                        Howtouse = x.Howtouse,
                        Name = x.Name,
                        Price = x.Price,
                        Source = source,
                        Type = x.Type,
                        Url = x.Url,
                        Weight = wp ? dWeight : 0,
                        Volume = vp ? dVolume : 0,
                        Extra = Newtonsoft.Json.JsonConvert.SerializeObject(x.Extra_attributes),
                    });
                });
                var excl = _railwayContext.Products.Where(x => productList.Select(x => x.Url).ToList().Contains(x.Url)).Select(x => x.Url).ToList();
                excl.RemoveAll(item => string.IsNullOrWhiteSpace(item));
                var resex = productList.Except(productList.Where(x => excl.Contains(x.Url))).ToList();
                await _railwayContext.Products.AddRangeAsync(resex);
                var count = await _railwayContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }

    }
}