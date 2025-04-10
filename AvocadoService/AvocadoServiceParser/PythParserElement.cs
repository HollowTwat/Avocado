using System.Collections.Generic;

namespace AvocadoService.AvocadoServiceParser
{
    public class PythParserElement
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public string Weight { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string Consist { get; set; }
        public decimal? Price { get; set; }
        public string Brandinfo { get; set; }
        public string Url { get; set; }
        public string Howtouse { get; set; }
        public string Volume { get; set; }
        public Dictionary<string,string> Extra_attributes { get; set; }
    }
}
