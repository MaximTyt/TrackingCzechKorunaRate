using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Services.Utils
{
    public class CNBRequest : ICNBRequest
    {
        private readonly BaseAddressSetting _baseAddress;
        private HttpClient sharedClient;
        public CNBRequest(IOptions<BaseAddressSetting> baseAddress)
        {
            _baseAddress = baseAddress.Value;
            sharedClient = new()
            {
                BaseAddress = new Uri(_baseAddress.BaseAddressURI),
            };
        }
         
        public async Task<ConcurrentDictionary<string, decimal>> GetRateByDateAsync(string date)
        {
            using HttpResponseMessage response = await sharedClient.GetAsync($"?date={date}");
            if (response.IsSuccessStatusCode)
            {
                return Utils.ParseData(response.Content.ReadAsStringAsync());
            }
            return [];
        }

    }
}
