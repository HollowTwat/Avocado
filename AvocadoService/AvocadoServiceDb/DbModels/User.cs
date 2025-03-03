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
        public string Bodyskintype { get; set; }
        public string Bodyskinsensitivity { get; set; }
        public string Bodyskincondition { get; set; }
        public string Bodyhairissues { get; set; }
        public string Bodyattentionareas { get; set; }
        public string Bodygoals { get; set; }
        public string Faceskintype { get; set; }
        public string Faceskincondition { get; set; }
        public string Faceskinissues { get; set; }
        public string Faceskingoals { get; set; }
        public string Hairscalptype { get; set; }
        public string Hairthickness { get; set; }
        public string Hairlength { get; set; }
        public string Hairstructure { get; set; }
        public string Haircondition { get; set; }
        public string Hairgoals { get; set; }
        public string Hairwashingfrequency { get; set; }
        public string Haircurrentproducts { get; set; }
        public string Hairproducttexture { get; set; }
        public string Hairsensitivity { get; set; }
        public string Hairstylingtools { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
    }
}
