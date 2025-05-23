using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FMS.Server.Data;
using FMS.Server.Models;

namespace FMS.Server.Controllers;

[ApiController]
[Route("api/revenue")]
public class RevenueController : ControllerBase
{
    private readonly AppDbContext _context;

    public RevenueController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromBody] List<RevenueRecord> records)
    {
        await _context.RevenueRecords.AddRangeAsync(records);
        await _context.SaveChangesAsync();
        return Ok(new { message = "上传成功", count = records.Count });
    }

    [HttpGet("query")]
    public async Task<IActionResult> Query([FromQuery] DateOnly date, [FromQuery] string? doctor = null)
    {
        var start = date.ToDateTime(TimeOnly.MinValue);
        var end = date.ToDateTime(TimeOnly.MaxValue);
        var query = _context.RevenueRecords.Where(r => r.Date >= start && r.Date <= end);

        if (!string.IsNullOrWhiteSpace(doctor))
            query = query.Where(r => r.Doctor == doctor);

        var result = await query
            .GroupBy(r => r.ItemType)
            .Select(g => new {
                ItemType = g.Key,
                TotalAmount = g.Sum(x => x.Amount)
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("statistics/monthly")]
    public async Task<IActionResult> GetMonthlyStatistics([FromQuery] int year)
    {
        var result = await _context.RevenueRecords
            .Where(r => r.Date.Year == year)
            .GroupBy(r => r.Date.Month)
            .Select(g => new {
                Month = g.Key,
                TotalAmount = g.Sum(x => x.Amount),
                ByDoctor = g.GroupBy(x => x.Doctor)
                            .Select(d => new { Doctor = d.Key, Amount = d.Sum(x => x.Amount) })
            })
            .OrderBy(r => r.Month)
            .ToListAsync();

        return Ok(result);
    }
}
