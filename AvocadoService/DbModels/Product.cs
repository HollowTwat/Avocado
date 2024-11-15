using System;
using System.Collections.Generic;

namespace AvocadoService.DbModels
{
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brend { get; set; }
        public string Type { get; set; }
        public float? Weight { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string Consist { get; set; }
    }
}
