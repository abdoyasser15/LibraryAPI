using Library.Core.ServiceContract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library.Service
{
    public class ResponseCashService : IResponseCashService
    {
        private readonly IDatabase _database;
        public ResponseCashService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task CashResponseAsync(string CashKey, object Response, TimeSpan TimeToLive)
        {
            if (Response is null)
                return;
            var serializedResponse = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var serializedData = JsonSerializer.Serialize(Response, serializedResponse);
            await _database.StringSetAsync(CashKey, serializedData, TimeToLive);
        }

        public async Task<string?> GetCashResponseAsync(string CashKey)
        {
            var cachedResponse = await _database.StringGetAsync(CashKey);

            if (cachedResponse.IsNullOrEmpty)
                return null;
            return cachedResponse.ToString();
        }
    }
}
