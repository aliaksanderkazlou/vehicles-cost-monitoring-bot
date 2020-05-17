using System;

namespace VehiclesCostMonitoring.Bot.Services.Impl.Onliner
{
    public class OnlinerResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public Model Model { get; set; }
        public Price Price { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? last_up_at { get; set; }
    }
}