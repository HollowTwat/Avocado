using AvocadoDb.DbModels;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json.Linq;

namespace AvocadoParser
{
    public class ParserHelper
    {
        public async Task<List<Product>> ParceElements(List<string> productsUrls)
        {
            var res = new List<Product>();
  
            foreach (var productsUrl in productsUrls)
            {
                Thread.Sleep(50);
                res.Add(await ParseElement(productsUrl));
            }
            //var b = Newtonsoft.Json.JsonConvert.SerializeObject(res);
            return res;

        }
      
        private async Task<string> GetPage(string siteUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                var res = await httpClient.GetAsync(siteUrl);
                var html = await res.Content.ReadAsStringAsync();
                //var t = Path.GetTempFileName();
                //File.WriteAllText(t, html);
                return html;
            }
        }
        public async Task<List<string>> GetProductUrlsList(int multi, string Cat)
        {
            var res = new List<string>();
            for (int i = 1; i < multi; i++)
            {
                var siteUrl = $"https://api.rivegauche.ru/rg/v1/newRG/products/search?fields=FULL&currentPage={i}&pageSize=24&categoryCode={Cat}";
                Thread.Sleep(100);
                var html = await GetPage(siteUrl);
                //HtmlDocument htmlSnippet = new HtmlDocument();
                //htmlSnippet.LoadHtml(html);
                //var attrs = htmlSnippet.DocumentNode.SelectNodes("//product-item/div/a");
                //var r = attrs.Select(x => x.GetAttributeValue("href", string.Empty));
                var jj = JObject.Parse(html);
                var r = jj["results"].Select(x => x["url"].Value<string>());
                res.AddRange(r.ToList());
            }
            return res;
        }
        private async Task<string> GetElementHTML(string finalturl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                var res = await httpClient.GetAsync($"https://rivegauche.ru{finalturl}");
                var html = await res.Content.ReadAsStringAsync();
                //var t = Path.GetTempFileName();
                //File.WriteAllText(t, html);
                return html;
            }
        }
        private async Task<Product> ParseElement(string url)
        {

            var html = await GetElementHTML(url);
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
                        var description = k["description"];
                        var featurePairs = features?
         .GroupBy(obj => (string)obj["name"])
         .ToDictionary(
             group => group.Key,
             group => string.Join(", ", group.Select(obj => (string)obj["value"]))
         );

                        nnullcount = (name != null ? 1 : 0) + (features != null ? 1 : 0) + (price != null ? 1 : 0) +
                            (ingredients != null ? 1 : 0) + (brand != null ? 1 : 0) + (brandInfo != null ? 1 : 0);
                        if (nnullcount > 3)
                        {
                            ;
                            var productElement = new Product
                            {
                                Name = name?.Value<string>(),
                                Brand = brand?.Value<string>(),
                                Price = price == null ? null : decimal.Parse(price?["value"].Value<string>()),
                                Brandinfo = brandInfo?.Value<string>(),
                                Country = featurePairs?.ContainsKey("Производство") == true ? featurePairs?["Производство"] : null,
                                Weight = featurePairs?.ContainsKey("Объем, мл") == true ? (int.TryParse(featurePairs?["Объем, мл"], out var w) == true ? w : null) : null,
                                Type = featurePairs?.ContainsKey("Продукт") == true ? featurePairs["Продукт"] : null,
                                Consist = ingredients?.Value<string>(),
                                //HTML = html,
                                Description = description?.Value<string>(),
                                Url = url,
                            };
                            return productElement;
                        }
                    }

                }
                return new Product {/* HTML = html, */Url = url };
            }
            catch (Exception ex) { return new Product { /*HTML = html,*/ Url = url }; }
        }
    }
}
