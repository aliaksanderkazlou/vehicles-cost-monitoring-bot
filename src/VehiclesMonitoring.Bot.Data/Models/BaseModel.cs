using MongoDB.Bson.Serialization.Attributes;

namespace VehiclesMonitoring.Bot.Data.Models
{
    public abstract class BaseModel
    {
        [BsonId]
        public string Id { get; set; }
    }
}