using System;
using System.Collections.Generic;
using System.Linq;
using VehiclesCostMonitoring.Bot.Services.Attributes;
using VehiclesCostMonitoring.Bot.Services.Exceptions;

namespace VehiclesCostMonitoring.Bot.Services
{
    public static class ProviderTypeStorage
    {
        private static Dictionary<string, Type> _dictionary;
        
        public static void Initialize()
        {
            var type = typeof(IProvider);
            var handlers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && p.CustomAttributes.Any())
                .ToArray();

            _dictionary = handlers.ToDictionary(handler =>
            {
                var attribute = handler.GetCustomAttributes(true).Single() as ProviderAttribute;

                return (attribute?.Url);
            }, handler => handler);
        }

        public static IEnumerable<Type> GetAll()
        {
            return _dictionary.Values;
        }

        public static Type GetByUrl(string url)
        {
            var key = _dictionary.Keys.SingleOrDefault(url.StartsWith);

            if (key is null)
            {
                throw new ProviderException(ProviderExceptionType.ProviderNotFound);
            }

            return _dictionary[key];
        }
    }
}