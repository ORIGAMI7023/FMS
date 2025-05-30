
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
    /// ��ȡ��ǰ�·�ҽ��Ӫ�ջ��ܡ�
    /// </summary>
    public async Task<DoctorMonthlySummary?> GetDoctorSummaryCurrentMonthAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("/api/revenue/doctors/summary/currentMonth");
        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            DoctorMonthlySummary? dto = JsonConvert.DeserializeObject<DoctorMonthlySummary>(json);
            return dto;
        }

        throw new HttpRequestException($"��ȡҽ������ʧ��: {response.StatusCode} - {response.ReasonPhrase}");
    }

    /// <summary>
    /// ��ȡָ���·�ҽ��Ӫ�ջ��ܡ�
    /// year / month ���1-12���������߼�ͬ currentMonth��
    /// </summary>
    [HttpGet("doctors/summary")]
    public async Task<ActionResult<DoctorMonthlySummaryDto>> GetDoctorsSummaryByMonth(
        [FromQuery] int year,
        [FromQuery] int month)
    {
        if (month < 1 || month > 12) return BadRequest("month ����Ϊ 1-12");

        DateOnly monthStart = new DateOnly(year, month, 1);
        DateOnly monthEnd = monthStart.AddMonths(1);

        IQueryable<RevenueRecord> baseQuery = _context.RevenueRecords
            .Where(r => r.Date >= monthStart && r.Date < monthEnd)
            .Where(r => !r.IsExcludedFromSummary);

        int businessDays = await baseQuery
            .Where(r => !r.IsVisitCount)
            .Select(r => r.Date)
            .Distinct()
            .CountAsync();

        List<DoctorMonthlySummaryDto.DoctorRow> rows = await baseQuery
            .GroupBy(r => r.Owner)
            .Select(g => new DoctorMonthlySummaryDto.DoctorRow
            {
                Owner = g.Key,
                TotalRevenue = g.Where(r => !r.IsVisitCount)
                                .Sum(r => r.ItemType == "�˷�" ? -r.Value : r.Value),
                TotalVisits = (int)g.Where(r => r.IsVisitCount)
                                    .Sum(r => r.Value)
            })
            .ToListAsync();

        rows = rows.OrderByDescending(r => r.TotalRevenue).ToList();

        DoctorMonthlySummaryDto dto = new DoctorMonthlySummaryDto
        {
            BusinessDays = businessDays,
            TotalMonthlyRevenue = rows.Sum(r => r.TotalRevenue),
            TotalMonthlyVisits = rows.Sum(r => r.TotalVisits),
            Doctors = rows
        };

        return Ok(dto);
    }

}
