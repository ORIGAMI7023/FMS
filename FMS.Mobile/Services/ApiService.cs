using FMS.Mobile.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace FMS.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7050/");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<RevenueRecord>> GetDailyRevenueAsync(DateOnly date)
    {
        var url = $"api/revenue/query?date={date:yyyy-MM-dd}";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return new();

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<RevenueRecord>>(json) ?? new();
    }
}