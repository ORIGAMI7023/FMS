using FMS.Mobile.Models;
using Newtonsoft.Json;

namespace FMS.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://你的服务器地址") // 替换为实际地址
        };
    }

    public async Task<List<RevenueRecord>> GetDailyRevenueAsync(DateOnly date)
    {
        try
        {
            var url = $"/api/revenue/query?date={date:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] API Response: {json}");
                return JsonConvert.DeserializeObject<List<RevenueRecord>>(json);
            }
            else
            {
                Console.WriteLine($"[ERROR] HTTP {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EXCEPTION] {ex.Message}");
        }

        return new List<RevenueRecord>();
    }
}