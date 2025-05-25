using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FMS.Server.Data;
using FMS.Server.Models;
using FMS.Server.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FMS.Server.Controllers;

[ApiController]
[Route("api/revenue")]
public class RevenueController : ControllerBase
{
    private readonly AppDbContext _context;

    public RevenueController(AppDbContext context) { _context = context; }

    /// <summary>
    /// import接口，用于从JSON格式批量导入收入记录数据。
    /// </summary>
    [HttpPost("import")]
    public async Task<IActionResult> ImportFromJson([FromBody] List<RevenueRecord> records)
    {
        //Console.WriteLine("---- 收到 POST /import 请求，共有记录数：" + records?.Count);

        if (records == null || records.Count == 0)
            return BadRequest(new { message = "上传数据为空" });

        foreach (var record in records)
        {   
            record.Id = Guid.NewGuid(); // 生成guid主键
            if (string.IsNullOrWhiteSpace(record.Source))
                record.Source = "Admin";  // 如果Soruce字段为空，默认标注来源为Admin
        }

        try
        {
            await _context.RevenueRecords.AddRangeAsync(records);
            await _context.SaveChangesAsync();

            return Ok(new { message = "导入成功", count = records.Count });  // 返回导入成功的记录数
        }
        catch (DbUpdateException ex)
        {
            // 数据库更新失败（比如外键、唯一键等问题）
            return StatusCode(500, new { message = "数据库更新失败", error = ex.Message });
        }
        catch (Exception ex)
        {
            // 其他未处理异常
            return StatusCode(500, new { message = "导入失败", error = ex.Message });
        }
    }

    /// <summary>
    /// query接口，用于查询收入记录数据。
    /// </summary>
    [HttpGet("query")]
    public async Task<IActionResult> Query(
    [FromQuery] DateOnly? start = null,
    [FromQuery] DateOnly? end = null,
    [FromQuery] string? owner = null,
    [FromQuery] string? source = null,
    [FromQuery] string? itemType = null)
    {
        var query = _context.RevenueRecords.AsQueryable();

        if (start.HasValue)
            query = query.Where(r => r.Date >= start.Value);

        if (end.HasValue)
            query = query.Where(r => r.Date <= end.Value);

        if (!string.IsNullOrWhiteSpace(owner))
            query = query.Where(r => r.Owner == owner);

        if (!string.IsNullOrWhiteSpace(source))
            query = query.Where(r => r.Source == source);

        if (!string.IsNullOrWhiteSpace(itemType))
            query = query.Where(r => r.ItemType == itemType);

        var result = await query
            .OrderBy(r => r.Date)
            .ToListAsync();

        return Ok(result);
    }

    /// <summary>
    /// 获取指定月份的收入统计摘要信息。
    /// </summary>
    /// <returns>包含 指定月度总营收，指定日期营收，指定月月均收入 </returns>
    [HttpGet("statistics/monthly/summary")]
    public async Task<ActionResult<MonthlySummaryDto>> GetMonthlySummary(DateOnly date)
    {
        var targetMonthStart = new DateOnly(date.Year, date.Month, 1);  // 获取目标月的第一天
        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);  // 获取该月的总天数
        var targetMonthEnd = targetMonthStart.AddDays(daysInMonth);  // 即下月1号，用于 "<" 判断

        // 获取本月记录
        var recordsInMonth = await _context.RevenueRecords
            .Where(r =>
                r.Date >= targetMonthStart &&
                r.Date < targetMonthEnd &&
                !r.IsExcludedFromSummary &&
                !r.IsVisitCount)
            .ToListAsync();

        // 封装计算逻辑：退费为负数
        decimal AdjustedValue(RevenueRecord r) =>
            r.ItemType == "退费" ? -r.Value : r.Value;

        var totalMonthly = recordsInMonth.Sum(r => AdjustedValue(r));

        var totalToday = recordsInMonth
            .Where(r => r.Date == date)
            .Sum(r => AdjustedValue(r));

        var averageDaily = daysInMonth > 0 ? totalMonthly / daysInMonth : 0;

        var result = new MonthlySummaryDto
        {
            TotalMonthly = totalMonthly,
            TotalToday = totalToday,
            AverageDaily = Math.Round(averageDaily, 2)
        };

        return Ok(result);
    }




}
