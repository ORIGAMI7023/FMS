
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

    public async Task<List<MonthlyAmountStatistics>> GetMonthlyAmountStatisticsAsync(int year, int month)
    {
        var response = await _httpClient.GetAsync($"/api/revenue/statistics/monthly/amount?year={year}&month={month}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MonthlyAmountStatistics>>(json);
        }
        return new List<MonthlyAmountStatistics>();
    }

    public async Task<List<MonthlyVisitCountStatistics>> GetMonthlyVisitCountStatisticsAsync(int year, int month)
    {
        var response = await _httpClient.GetAsync($"/api/revenue/statistics/monthly/visitcount?year={year}&month={month}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MonthlyVisitCountStatistics>>(json);
        }
        return new List<MonthlyVisitCountStatistics>();
    }
}
