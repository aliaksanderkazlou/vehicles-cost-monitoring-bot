namespace VehiclesMonitoring.Bot.Data.Models
{
    public class User : BaseModel
    {
        public long ChatId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string Status { get; set; }
        
        public string[] VehicleIds { get; set; }
    }
}