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

    // 原收入明细接口
    public async Task<List<RevenueRecord>> GetDailyRevenueAsync(DateTime date)
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

    // 新增月度统计接口
    public async Task<List<ItemTypeStatistics>> GetMonthlyStatisticsAsync(int year, int month)
    {
        try
        {
            var url = $"/api/revenue/statistics/monthly?year={year}&month={month}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Monthly Statistics: {json}");
                return JsonConvert.DeserializeObject<List<ItemTypeStatistics>>(json);
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

        return new List<ItemTypeStatistics>();
    }

    internal async Task<IEnumerable<RevenueRecord>> GetDailyRevenueAsync(DateOnly selectedDate)
    {
        throw new NotImplementedException();
    }
}