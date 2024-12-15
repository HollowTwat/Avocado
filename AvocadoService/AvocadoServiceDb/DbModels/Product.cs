using System;
using System.Collections.Generic;

namespace AvocadoService.AvocadoServiceDb.DbModels
{
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public decimal? Weight { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string Consist { get; set; }
        public decimal? Price { get; set; }
        public string Brandinfo { get; set; }
        public string Url { get; set; }
        public string Howtouse { get; set; }
        public decimal? Volume { get; set; }
        public string Source { get; set; }
    }
}
