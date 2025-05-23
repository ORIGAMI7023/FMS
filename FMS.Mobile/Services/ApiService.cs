using FMS.Mobile.Models;
using Newtonsoft.Json;

namespace FMS.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
#if WINDOWS
        _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:7050") };
#elif ANDROID
        _httpClient = new HttpClient { BaseAddress = new Uri("http://192.168.50.203:7050") };
#else
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:7050") };
#endif
    }

    // 月度统计接口（示例，按实际接口调整参数即可）
    public async Task<List<ItemTypeStatistics>> GetStatisticsAsync(DateTime selectedDate)
    {
        var url = $"/api/revenue/statistics/monthly?year={selectedDate.Year}&month={selectedDate.Month}";
        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("[DEBUG] 返回JSON: " + json);
            return JsonConvert.DeserializeObject<List<ItemTypeStatistics>>(json);
        }
        return new List<ItemTypeStatistics>();
    }
}