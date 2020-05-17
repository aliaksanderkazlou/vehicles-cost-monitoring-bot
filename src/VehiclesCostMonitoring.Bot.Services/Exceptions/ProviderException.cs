using System;

namespace VehiclesCostMonitoring.Bot.Services.Exceptions
{
    public class ProviderException : Exception
    {
        public ProviderExceptionType Type { get; }
        
        public ProviderException(string message) : base(message) {}
        
        public ProviderException(string message, Exception innerException) : base(message, innerException) {}

        public ProviderException(ProviderExceptionType type)
        {
            Type = type;
        }
    }
}