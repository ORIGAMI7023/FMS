    using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FMS.Server.Data;
using FMS.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
        var query = _context.RevenueRecords.Where(r => r.Date == date);

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
    public async Task<IActionResult> GetMonthlyStatistics(int year, int month)
    {
        var result = await _context.RevenueRecords
            .Where(r => r.Date.Year == year && r.Date.Month == month)
            .GroupBy(r => r.ItemType)
            .Select(g => new ItemTypeStatistics
            {
                itemType = g.Key,
                totalAmount = g.Sum(r => r.Amount)
            })
            .ToListAsync();

        return Ok(result);
    }


    [HttpPost("import")]
    public async Task<IActionResult> ImportRevenue(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("文件为空");

        var records = new List<RevenueRecord>();

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            try
            {
                var date = DateOnly.FromDateTime(DateTime.Parse(row.Cell(1).GetString()));
                var doctor = row.Cell(2).GetString();
                var department = row.Cell(3).GetString();
                var itemType = row.Cell(4).GetString();
                var amount = row.Cell(5).GetValue<decimal>();

                records.Add(new RevenueRecord
                {
                    Id = Guid.NewGuid(),
                    Date = date,
                    Doctor = doctor,
                    Department = department,
                    ItemType = itemType,
                    Amount = amount,
                    Source = "Excel"
                });
            }
            catch
            {
                // 忽略错误行
            }
        }

        await _context.RevenueRecords.AddRangeAsync(records);
        await _context.SaveChangesAsync();

        return Ok(new { message = "导入成功", count = records.Count });
    }
}
