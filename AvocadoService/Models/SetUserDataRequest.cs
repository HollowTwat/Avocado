namespace AvocadoService.Models
{
    public class SetUserDataRequest
    {
        public string tg_id { get; set; }
        public UserData user_data { get; set; }

    }
    public class UserData
    {
        public string age { get; set; }
        public string gender { get; set; }
        public string location { get; set; }
        public string allergy { get; set; }
        public string lifestyle { get; set; }
        public string phototype { get; set; }
        public string activity { get; set; }
        public string water_intake { get; set; }
        public string stress { get; set; }
        public string habits { get; set; }
        
    }
}
