using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyNozbe.API.E2ETests
{
    public static class ResponseHelper
    {
        public static async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(stringResponse);
            return result;
        }
    }
}