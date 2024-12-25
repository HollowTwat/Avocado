using AvocadoService.AvocadoServiceDb.DbModels;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AvocadoService.AvocadoServiceParser
{
    public class FRESHParserHelper
    {
        private string _source { get; set; } = "FRESH";

        public async Task<List<Product>> GetProductList(int listFrom, int listTo, string Cat)
        {
            var res = new List<Product>();
            for (int i = listFrom; i < listTo; i++)
            {
                var siteUrl = $"https://api.4fresh.ru/api/v1/catalog/product-offers/list?categoryId={Cat}&sortBy=popular&page={i}&limit=24";
                Thread.Sleep(100);
                var html = await GetPage(siteUrl);
                //HtmlDocument htmlSnippet = new HtmlDocument();
                //htmlSnippet.LoadHtml(html);
                //var attrs = htmlSnippet.DocumentNode.SelectNodes("//product-item/div/a");
                //var r = attrs.Select(x => x.GetAttributeValue("href", string.Empty));
                var jj = JObject.Parse(html);
                var r = jj["data"]["items"];
                for (int j = 0; j < r.Count(); j++)
                {
                    var item = await GetElementHTML(r[j]["code"].ToString());
                    var element = await ParseElements(item["data"]);
                    res.AddRange(element);
                }
            }
            return res;
        }

        private async Task<List<Product>> ParseElements(JToken element)
        {
            try
            {
                var res=new List<Product>();
                var price = element["price"]?["sellPrice"];
                var offers = element["product"]["offers"];
           
                var propers = element["propertiesValues"].Children<JObject>()
              .ToDictionary(
                  item => item["title"]?.ToString(),
                  item => item["value"]?.ToString());
                foreach (var offer in offers)
                {
                    string volume = null;
                    var volumeToken = offer["propertiesValues"].Where(x => x["title"]?.ToString() == "Объем").ToList().SingleOrDefault();
                    if (volumeToken != null)
                    {
                        volume = volumeToken["value"]?.ToString();
                        volume = volume.Replace("мл", string.Empty);
                        volume = volume.Replace("л", "000");
                    }
                    var type = element["breadcrumbs"]?[3]?["code"].ToString();
                    if (string.IsNullOrEmpty(type))
                        type = "Витамины и БАДы";
                    var productElement = new Product
                    {
                        Name = element["title"]?.Value<string>(),
                        Brand = propers.ContainsKey("Бренд") ? propers["Бренд"] : string.Empty,
                        Price = price == null ? null : decimal.Parse(price.Value<string>()),
                        Brandinfo = string.Empty,
                        Country = propers.ContainsKey("Страна производства") ? propers["Страна производства"] : string.Empty,
                        Volume =  string.IsNullOrEmpty(volume) == false ? int.TryParse(volume, out var w) == true ? w : null : null,
                        Type = type,
                        Consist = propers.ContainsKey("Состав") ? propers["Состав"] : string.Empty,
                        //HTML = html,
                        Description = element["description"]?.ToString(),
                        Url = element["code"].ToString(),
                        Howtouse = propers.ContainsKey("Способ применения") ? propers["Способ применения"] : string.Empty,
                        Source = _source
                    };
                    productElement.Name += $",{productElement.Brand},{productElement.Volume}";
                    res.Add( productElement);
                }
                return res;
            }
            catch (Exception ex)
            {
                return new List<Product>();
            }
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
        private async Task<JObject> GetElementHTML(string elementCode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                var res = await httpClient.GetAsync($"https://api.4fresh.ru/api/v1/catalog/product-offers/detail/{elementCode}");
                var html = await res.Content.ReadAsStringAsync();
                //var t = Path.GetTempFileName();
                //File.WriteAllText(t, html);
                return JObject.Parse(html);
            }
        }
    }
}
