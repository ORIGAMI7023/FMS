
using FMS.Mobile.Models;
using Newtonsoft.Json;

namespace FMS.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
#if WINDOWS
        _httpClient = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:7050") };
#elif ANDROID
        _httpClient = new HttpClient { BaseAddress = new Uri("http://192.168.50.203:7050") };
#else
        _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7051") };
#endif
    }

    /// <summary>
    /// 获取指定年月的月度收入摘要
    /// </summary>
    public async Task<MonthlySummary?> GetMonthlySummaryAsync(DateOnly date)
    {
        var response = await _httpClient.GetAsync($"/api/revenue/home/summary?date={date}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MonthlySummary>(json);
        }
        else
        {
            throw new HttpRequestException($"请求失败: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }

}
