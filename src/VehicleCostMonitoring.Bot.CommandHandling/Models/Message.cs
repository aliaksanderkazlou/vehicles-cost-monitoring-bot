namespace VehicleCostMonitoring.Bot.CommandHandling.Models
{
    public class Message
    {
        public string Text { get; set; }

        public UserInfo UserInfo { get; set; }

        public ChatInfo ChatInfo { get; set; }
    }
}