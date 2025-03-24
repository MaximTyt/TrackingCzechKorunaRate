using System.Collections.Concurrent;

namespace Services.Utils
{
    public interface ICNBRequest
    {
        public Task<ConcurrentDictionary<string, decimal>> GetRateByDateAsync(string date);
    }
}
