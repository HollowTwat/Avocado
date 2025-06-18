using AvocadoService.AvocadoServiceDb.DbModels;
using AvocadoService.AvocadoServiceParser;
using AvocadoService.Helpers;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
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
                File.WriteAllText("output.json", Newtonsoft.Json.JsonConvert.SerializeObject(table));

                Xunit.Assert.True(true);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        [Fact]
        public async Task DuplicateURLDell()
        {
            try
            {
                var duplicates = await _railwayContext.Products
      .GroupBy(p => p.Url)
      .Where(g => g.Count() > 1)
      .Select(g => new
      {
          Url = g.Key,
          MaxId = g.Max(x => x.Id),
          AllIds = g.Select(x => x.Id).ToList()
      })
      .ToListAsync();

                int i = 0;
                // Удаляем все дубликаты, кроме записи с минимальным ID
                foreach (var group in duplicates)
                {
                    var idsToDelete = group.AllIds.Where(id => id != group.MaxId).ToList();
                    var productsToDelete = await _railwayContext.Products
                        .Where(p => idsToDelete.Contains(p.Id))
                        .ToListAsync();

                    _railwayContext.Products.RemoveRange(productsToDelete);
                    i++;
                    if (i > 2000)
                    {
                        await _railwayContext.SaveChangesAsync();
                        i = 0;
                    }
                }


                Xunit.Assert.True(true);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task DuplicateNameDell()
        {
            try
            {
                var duplicates = await _railwayContext.Products/*.Where(x => x.Source == "GOLD")*/
      .GroupBy(p => p.Name)
      .Where(g => g.Count() > 1)
      .Select(g => new
      {
          Name = g.Key,
          MaxId = g.Max(x => x.Id),
          AllIds = g.Select(x => x.Id).ToList(),
          //URL = g.Select(x => x.Url).ToList(),
          //Consist=g.Select(x=>x.Consist),
      })
      .ToListAsync();

                // Удаляем все дубликаты, кроме записи с минимальным ID
                foreach (var group in duplicates)
                {
                    var idsToDelete = group.AllIds.Where(id => id != group.MaxId).ToList();
                    var productsToDelete = await _railwayContext.Products
                        .Where(p => idsToDelete.Contains(p.Id))
                        .ToListAsync();

                    _railwayContext.Products.RemoveRange(productsToDelete);
                }

                await _railwayContext.SaveChangesAsync();
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
                var path = @"C:\\ReposMy\biodepo_products_data_improved.json";
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
        //[Fact]
        public async Task ParceNATINUELTest()
        {
            try
            {
                var source = "NATINUEL";
                var path = @"C:\\ReposMy\natinuel.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceFOAMSTORETest()
        {
            try
            {
                var source = "FOAMSTORE";
                var path = @"C:\\ReposMy\foamstore.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceBOXTest()
        {
            try
            {
                var source = "BOX";
                var path = @"C:\\ReposMy\exel.json";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }

        //[Fact]
        public async Task ParceSKINPROTest()
        {
            try
            {
                var source = "SKINPRO";
                var path = @"C:\\ReposMy\skinpro_products.json";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceROCHERTest()
        {
            try
            {
                var source = "ROCHER";
                var path = @"C:\\ReposMy\yves_rocher_products.json";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceYARKOSTTest()
        {
            try
            {
                var source = "YARKOST";
                var path = @"C:\\ReposMy\yarkostorganic.jsonl";
                var res = await ParceFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceLETUTest()
        {
            try
            {
                var source = "LETU";
                var path = @"C:\\ReposMy\letu_combined_data.json";
                var res = await ParceBIGFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceLETU2Test()
        {
            try
            {
                var source = "LETU";
                var path = @"C:\\ReposMy\letu_combined_data_2.json";
                var res = await ParceBIGFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceLETU3Test()
        {
            try
            {
                var source = "LETU";
                var path = @"C:\\ReposMy\letu_combined_data_3.json";
                var res = await ParceBIGFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceLETU4Test()
        {
            try
            {
                var source = "LETU";
                var path = @"C:\\ReposMy\letu_combined_data_4.json";
                var res = await ParceBIGFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceLETU5Test()
        {
            try
            {
                var source = "LETU";
                var path = @"C:\\ReposMy\letu_combined_data_5.json";
                var res = await ParceBIGFileTest(source, path);
                Xunit.Assert.True(res);
            }
            catch (Exception ex) { Xunit.Assert.Fail(); }
        }
        //[Fact]
        public async Task ParceLETU6Test()
        {
            try
            {
                var source = "LETU";
                var path = @"C:\\ReposMy\letu_combined_data_6.json";
                var res = await ParceBIGFileTest(source, path);
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

        private async Task<bool> ParceBIGFileTest(string source, string path)
        {
            try
            {
                var productList = new List<Product>();
                var existingUrls = _railwayContext.Products
                    .Select(x => x.Url)
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .ToList();

                // Потоковое чтение и десериализация JSON
                using (var fileStream = File.OpenRead(path))
                using (var streamReader = new StreamReader(fileStream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new Newtonsoft.Json.JsonSerializer();

                    // Читаем массив поэлементно
                    while (await jsonReader.ReadAsync())
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject)
                        {
                            var element = serializer.Deserialize<PythParserElement>(jsonReader);

                            // Обработка элемента
                            var wp = decimal.TryParse(element.Weight, out decimal dWeight);
                            var vr = element?.Volume?.Trim()?.Replace(" ", "")?.Replace("мл", string.Empty)?.Replace("л", "000");
                            var vp = decimal.TryParse(vr, out decimal dVolume);

                            productList.Add(new Product
                            {
                                Brand = element.Brand,
                                Brandinfo = element.Brandinfo,
                                Consist = element.Consist,
                                Country = element.Country,
                                Description = element.Description,
                                Howtouse = element.Howtouse,
                                Name = element.Name,
                                Price = element.Price,
                                Source = source,
                                Type = element.Type,
                                Url = element.Url,
                                Weight = wp ? dWeight : 0,
                                Volume = vp ? dVolume : 0,
                                Extra = JsonConvert.SerializeObject(element.Extra_attributes),
                            });

                            // Очищаем список каждые N элементов, чтобы не перегружать память
                            if (productList.Count >= 2000)
                            {
                                await ProcessBatch(productList, existingUrls);
                                productList.Clear();
                            }
                        }
                    }
                }

                // Обработка оставшихся элементов
                if (productList.Count > 0)
                {
                    await ProcessBatch(productList, existingUrls);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Логируем ошибку для отладки
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        // Метод для обработки пакета записей и сохранения в БД
        private async Task ProcessBatch(List<Product> products, List<string> existingUrls)
        {
            //existingUrls = _railwayContext.Products.AsNoTracking()
            //       .Select(x => x.Url)
            //       .Where(url => !string.IsNullOrWhiteSpace(url))
            //       .ToList();
            var newProducts = products
                .Where(p => !existingUrls.Contains(p.Url))
                .ToList();

            if (newProducts.Any())
            {
                await _railwayContext.Products.AddRangeAsync(newProducts);
                await _railwayContext.SaveChangesAsync();
            }
        }
        //[Fact]
        public void ShouldReturnBrandNamesFromJson()
        {
            // Путь к JSON файлу
            var jsonFilePath = @"C:\\ReposMy\aaa.json";
            var jsonFilePath2 = @"C:\\ReposMy\aaa2.json";
            // Чтение и парсинг JSON файла
            var jsonData = File.ReadAllText(jsonFilePath);
            var brands = JsonConvert.DeserializeObject<List<BrandData>>(jsonData);
            var jsonData2 = File.ReadAllText(jsonFilePath2);
            var brands2 = JsonConvert.DeserializeObject<List<BrandData>>(jsonData2);
            // Извлечение значений поля navigationState без "/brand/"
            var brandNames = brands
                .SelectMany(brand => brand.Items)
                .Select(item => item.NavigationState.Split(new[] { "/brand/" }, StringSplitOptions.None).LastOrDefault())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToArray();
            var brandNames2 = brands2
               .SelectMany(brand2 => brand2.Items)
               .Select(item2 => item2.NavigationState.Split(new[] { "/brand/" }, StringSplitOptions.None).LastOrDefault())
               .Where(name2 => !string.IsNullOrEmpty(name2))
               .ToArray();
            var brandNamesRes = brandNames2.Except(brandNames).ToList();

            var rrr = Newtonsoft.Json.JsonConvert.SerializeObject(brandNamesRes);
            // Ожидаемая далее проверка (используется для наглядности, может быть заменена тестированием фактов)
            string[] expected = { "aa", "aaadesign" };

        }
        private class BrandData
        {
            [JsonProperty("items")]
            public List<Item> Items { get; set; }
        }

        private class Item
        {
            [JsonProperty("navigationState")]
            public string NavigationState { get; set; }
        }
    }
}