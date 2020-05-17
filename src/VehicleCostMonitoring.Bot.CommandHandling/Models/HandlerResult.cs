namespace VehicleCostMonitoring.Bot.CommandHandling.Models
{
    public class HandlerResult
    {
        public string Message { get; set; }

        public bool NeedKeyBoard { get; set; } = false;
        
        public string[] KeyBoardOptions { get; set; }
    }
}