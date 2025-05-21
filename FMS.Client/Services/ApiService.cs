using FMS.Client.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FMS.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _client = new();

        public async Task<bool> UploadDailyDataAsync(DailyRevenueUploadRequest data)
        {
            var res = await _client.PostAsJsonAsync("https://localhost:7050/api/revenue/daily", data);
            return res.IsSuccessStatusCode;
        }
    }
}