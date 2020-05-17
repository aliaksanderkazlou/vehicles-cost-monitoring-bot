namespace VehiclesMonitoring.Bot.Data.Models
{
    public class Vehicle : BaseModel
    {
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public bool IsArchived { get; set; }
        
        public string[] UserIds { get; set; }
        
        public string IdFromPlatform { get; set; }
        
        public string Url { get; set; }
        
        public Cost[] Costs { get; set; }
    }
}