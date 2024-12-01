using AvocadoDb.DbModels;
using AvocadoParser;
using AvocadoService.Helpers;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace NutriDbTest
{
    [TestFixture]
    public class UnitTest1
    {
        private Mock<railwayContext> _mockContext;
        private Mock<IServiceScopeFactory> _mockServiceScopeFactory;
        private Mock<IServiceScope> _mockScope;
        private Mock<IServiceProvider> _mockServiceProvider;
        private Mock<ILogger<NotificationHelper>> _mockLogger;

        [SetUp]
        public void Setup()
        {
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
        public async Task BaseParsceTest()
        {
            string productBaseUrl = "https://rivegauche.ru";
            try
            {
                using HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                var res = await httpClient.GetAsync("https://rivegauche.ru/category/kosmetsevtika?currentPage=6");
                var html = await res.Content.ReadAsStringAsync();
                //    var t = Path.GetTempFileName();
                //    File.WriteAllText(t, html);

                //var html = await File.ReadAllTextAsync("C:\\Users\\Thinkbook\\AppData\\Local\\Temp\\tmp3E51.tmp");
                HtmlDocument htmlSnippet = new HtmlDocument();
                htmlSnippet.LoadHtml(html);
                var attrs = htmlSnippet.DocumentNode.SelectNodes("//product-item/div/a");
                var r = attrs.Select(x => x.GetAttributeValue("href", string.Empty));
            }
            catch (Exception ex)
            {
                Xunit.Assert.True(false);
            }
        }

        [Fact]
        public async Task HelperTest()
        {
            //ParserHelper parserHelper = new ParserHelper();
            //var productsUrls = await parserHelper.GetProductUrlsList(200, "SkinCare_Body");
            //var productsUrlsDis = productsUrls.Distinct().ToList();
            //var ciclec = productsUrlsDis.Count() / 100;
            //for (var i = 0; i <= ciclec; i++)
            //{
            //    Console.WriteLine($"i={i}");
            //    List<string> urls;
            //    if (i != ciclec)
            //        urls = productsUrlsDis.GetRange(i * 100, 100);
            //    else
            //        urls = productsUrlsDis.GetRange(i * 100, productsUrlsDis.Count() - ciclec * 100);
            //    var products = await parserHelper.ParceElements(urls);
            //    var productsDisc = products.Distinct().ToList();
            //    await _mockContext.Products.AddRangeAsync(productsDisc);
            //    await _mockContext.SaveChangesAsync();
            //}
            Xunit.Assert.True(true);
        }
        [Fact]
        public async Task ParceFileTest()
        {
            var text = await System.IO.File.ReadAllTextAsync("");
            var res=Newtonsoft.Json.JsonConvert.DeserializeObject<List<Product>>(text);
        }
        [Fact]
        public async Task ElementParsceTest()
        {
            string url = "https://api.rivegauche.ru/rg/v1/newRG/products/search?fields=FULL&currentPage=1&pageSize=24&categoryCode=Cosmeceuticals";


            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            var res = await httpClient.GetAsync(url);
            var html = await res.Content.ReadAsStringAsync();

            try
            {
                HtmlDocument htmlSnippet = new HtmlDocument();
                htmlSnippet.LoadHtml(html);
                var sc = htmlSnippet.DocumentNode.SelectSingleNode("//script[@type='application/json']").InnerHtml;
                var jj = JObject.Parse(sc);
                foreach (var el in jj.Children())
                {
                    int nnullcount = 0;
                    var k = el.FirstOrDefault()?.FirstOrDefault()?.FirstOrDefault();
                    if (k != null)
                    {
                        var name = k["name"];
                        var features = k["features"];
                        var price = k["price"];
                        var ingredients = k["ingredients"];
                        var brand = k["brand"]?["name"];
                        var brandInfo = k["brand"]?["seoBrandDescription"];
                        var featurePairs = features?
         .GroupBy(obj => (string)obj["name"])
         .ToDictionary(
             group => group.Key,
             group => string.Join(", ", group.Select(obj => (string)obj["value"]))
         );
                        //            var featurePairs = features?
                        //.ToDictionary(
                        //    obj => (string)obj["name"],
                        //    obj => (string)obj["value"]);

                        nnullcount = (name != null ? 1 : 0) + (features != null ? 1 : 0) + (price != null ? 1 : 0) +
                            (ingredients != null ? 1 : 0) + (brand != null ? 1 : 0) + (brandInfo != null ? 1 : 0);
                        if (nnullcount > 3)
                        {
                            var productElement = new ProductElement
                            {
                                Name = name?.Value<string>(),
                                Brand = brand?.Value<string>(),
                                Price = price == null ? null : decimal.Parse(price?["value"].Value<string>()),
                                BrandInfo = brandInfo?.Value<string>(),
                                Country = featurePairs?.ContainsKey("Производство") == true ? featurePairs?["Производство"]: null,
                                Weight = featurePairs?.ContainsKey("Объем, мл") == true ? int.Parse(featurePairs?["Объем, мл"]):null,
                                 Type = featurePairs?.ContainsKey("Продукт") == true ? featurePairs["Продукт"]:null,
                                Consist = ingredients?.Value<string>(),
                                //HTML = html,
                                URL = url
                            };
                            // return productElement;
                        }
                    }

                }
                // return new ProductElement { HTML = html, URL = url };
            }
            catch (Exception ex) { }//            return new ProductElement { HTML = html, URL = url }; }
        }
    }
}