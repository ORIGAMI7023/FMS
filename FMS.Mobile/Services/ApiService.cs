
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
        _httpClient = new HttpClient { BaseAddress = new Uri("http://192.168.50.203:7050") };//̨ʽ������
        //_httpClient = new HttpClient { BaseAddress = new Uri("http://192.168.90.114:7050") };//�ʼǱ�����
        //_httpClient = new HttpClient { BaseAddress = new Uri("http://1.94.145.54:7050") };//����������
#else
        _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7051") };
#endif
    }

    /// <summary>
    /// ��ȡָ�����µ��¶�����ժҪ
    /// </summary>
    public async Task<MonthlySummary?> GetMonthlySummaryAsync(DateOnly date)
    {
        var response = await _httpClient.GetAsync($"/api/revenue/home/summary/monthly?date={date}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MonthlySummary>(json);
        }
        else
        {
            throw new HttpRequestException($"����ʧ��: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }

    /// <summary>
    /// ��ȡָ���·�ҽ��Ӫ�ջ��ܡ�
    /// </summary>
    public async Task<DoctorMonthlySummary?> GetDoctorSummaryAsync(int year, int month)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync($"/api/revenue/doctors/summary?year={year}&month={month}");
        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DoctorMonthlySummary>(json);
        }
        throw new HttpRequestException($"��ȡҽ������ʧ��: {response.StatusCode} - {response.ReasonPhrase}");
    }

}
