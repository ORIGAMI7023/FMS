using System.Net.Http;
using System.Net.Http.Json;

public class ApiService
{
    private readonly HttpClient _client = new();

    public async Task<bool> UploadRevenueAsync(RevenueRecord record)
    {
        var res = await _client.PostAsJsonAsync("https://localhost:7050/api/revenue", record);
        return res.IsSuccessStatusCode;
    }
}
