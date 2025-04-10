using AngleSharp.Common;
using AngleSharp.Html.Parser;
using AvocadoService.AvocadoServiceDb.DbModels;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AvocadoService.AvocadoServiceParser
{
    public class GOLDParserHelper
    {
        private string _source { get; set; } = "GOLD";

        public async Task<List<Product>> GetProductList(int listFrom, int listTo, string Cat)
        {
            var res = new List<Product>();
            for (int i = listFrom; i < listTo; i++)
            {
                var siteUrl = $"https://goldapple.ru/front/api/catalog/products?categoryId=1000000004&cityId=0c5b2444-70a0-4932-980c-b4dc0d3f02b5&pageNumber={i}";
                //Thread.Sleep(100);
                var html = await GetPage(siteUrl);
                //HtmlDocument htmlSnippet = new HtmlDocument();
                //htmlSnippet.LoadHtml(html);
                //var attrs = htmlSnippet.DocumentNode.SelectNodes("//product-item/div/a");
                //var r = attrs.Select(x => x.GetAttributeValue("href", string.Empty));
                var jj = JObject.Parse(html);
                var r = jj["data"]["products"];
                for (int j = 0; j < r.Count(); j++)
                {
                    var item = await GetElementJson(r[j]["url"].ToString());
                    var element = await ParseElements(item);
                    res.AddRange(element);
                }
            }
            return res;
        }

        private async Task<List<Product>> ParseElements(JsonDocument element)
        {
            try
            {
                var jel = element.RootElement.GetProperty("data");
                var name = jel.GetProperty("name").GetString();
                var isBrand = jel.TryGetProperty("brand", out JsonElement brand);
                var isProductType = jel.TryGetProperty("productType", out JsonElement pType);
                var isDesc = jel.TryGetProperty("productDescription", out JsonElement desc);
                var isVariant = jel.TryGetProperty("variants", out JsonElement variants);
                var isAttributes = jel.TryGetProperty("attributes", out JsonElement attributes);
                var isml = attributes.GetProperty("units").GetProperty("unit").GetString().Contains("мл");
                //var vola = attributes.EnumerateArray();

                var voll = attributes
        .GetProperty("units")
        .GetProperty("options")
        .EnumerateArray()
        .Select(option => option.GetProperty("value").GetString())
        .ToList();

                var variant = variants.EnumerateArray().FirstOrDefault();
                var actualPrice = variant.GetProperty("price")
                                .GetProperty("actual")
                                .GetProperty("amount")
                                .GetDecimal();
                var attributesList = desc
          .EnumerateArray()
          .Where(el => el.TryGetProperty("attributes", out _))
          .SelectMany(el => el.GetProperty("attributes").EnumerateArray())
          .Select(attr => (key: attr.GetProperty("key").GetString(),
                          value: attr.GetProperty("value").GetString()));

                //      var ar=    v.ToArray();
                //      Dictionary<string, string> dictionary = ar
                //.ToDictionary(
                //    element => element.GetProperty("text").GetString(),
                //    element => element.GetProperty("content").GetString()
                //);
                var contentDictionary = desc
           .EnumerateArray()
           .Where(el => el.TryGetProperty("text", out _) && el.TryGetProperty("content", out _))
           .ToDictionary(
               el => el.GetProperty("text").GetString(),
               el => el.GetProperty("content").GetString()
           );
                var res = new List<Product>();
                var productElement = new Product
                {
                    Name = jel.GetProperty("name").GetString(),
                    Brand = isBrand ? brand.GetString() : string.Empty,
                    Price = actualPrice,
                    Brandinfo = contentDictionary["о бренде"] ?? string.Empty,
                    //Country = propers.ContainsKey("Страна производства") ? propers["Страна производства"] : string.Empty,
                    //Volume = voll,
                    Type = isProductType ? pType.GetString() : string.Empty,
                    Consist = contentDictionary["состав"] ?? string.Empty,
                    ////HTML = html,
                    Description = contentDictionary["описание"] ?? string.Empty,
                    //Url = element["code"].ToString(),
                    Howtouse = contentDictionary["применение"] ?? string.Empty,
                    Source = _source
                };
                if (voll.Count <= 0)
                    voll.Add("0");
                foreach (var vole in voll)
                {
                    productElement.Volume = decimal.Parse(vole);
                    res.Add(productElement);
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
                httpClient.DefaultRequestHeaders.Add("X-GIB-GSSCw-goldapple-ru", "VnCX912INWyfXjtR2Wzy+z7Njwd7MSAErCdx8hwBOvNUfIifoYulaVf9fg23FlE6SRR5T4YYftdoz4oDcA1tejpcJhE+HMoR+pVM4qnE68jKfpNbo5Bam+1nKeksT6qmqpq+0thUeYtxWboBmP3u26xXXVNOiIhjCahzepsjGZouJmd+VrDoCZxwrjJ6Sl7nh+rEjPCAcvGrkhYvEP/OsCxHjspFHON02LE1X2tOQgt5CBfl0ZbwiksaRG/bLEEgbmQSHuiOiw==");
                httpClient.DefaultRequestHeaders.Add("X-GIB-FGSSCw-goldapple-ru", "5WSvecd94f8528654ea6276cb67fb3dae432999d");

                var res = await httpClient.GetAsync(siteUrl);
                var html = await res.Content.ReadAsStringAsync();
                //var t = Path.GetTempFileName();
                //File.WriteAllText(t, html);
                return html;
            }
        }

        private async Task<JsonDocument> GetElementJson(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                var res = await httpClient.GetAsync($"https://goldapple.ru{url}");
                var html = await res.Content.ReadAsStringAsync();
                //var t = Path.GetTempFileName();
                //File.WriteAllText(t, html);
                var parser = new HtmlParser();
                var document = parser.ParseDocument(html);
                var scriptContent = document.Scripts[2].TextContent;
                var jsonStr = scriptContent.Split(new string[] { "window.serverCache['productCard']=" }, StringSplitOptions.None)[1]
                                .TrimEnd('}');

                return System.Text.Json.JsonDocument.Parse(jsonStr.Substring(0, jsonStr.Length - 1));

            }
        }
    }
}
