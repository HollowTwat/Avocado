using System;
using System.Collections.Generic;

namespace AvocadoService.AvocadoServiceDb.DbModels
{
    public partial class User
    {
        public int Id { get; set; }
        public long UserTgId { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
        public string Allergy { get; set; }
        public string Lifestyle { get; set; }
        public string Activity { get; set; }
        public string WaterIntake { get; set; }
        public string Stress { get; set; }
        public string Habits { get; set; }
        public int? Age { get; set; }
    }
}
