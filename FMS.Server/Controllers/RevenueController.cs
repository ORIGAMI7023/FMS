using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FMS.Server.Data;
using FMS.Server.Models;
using FMS.Server.Models.Dtos;
using FMS.Server.Models.Dtos.FMS.Server.Models.Dtos;
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


    [HttpPost("import")]
    public async Task<IActionResult> ImportFromJson([FromBody] List<RevenueRecord> records)
    {
        if (records == null || records.Count == 0)
            return BadRequest(new { message = "上传数据为空" });

        // 标准化 Source
        foreach (var record in records)
        {
            if (string.IsNullOrWhiteSpace(record.Source))
                record.Source = "Admin";
        }

        // 查找所有与上传数据“字段完全一致”的已有记录
        var duplicateRecords = new List<RevenueRecord>();
        foreach (var r in records)
        {
            bool exists = await _context.RevenueRecords.AnyAsync(db =>
                db.Date == r.Date &&
                db.Value == r.Value &&
                db.Owner == r.Owner &&
                db.ItemType == r.ItemType &&
                db.IsVisitCount == r.IsVisitCount &&
                db.IsExcludedFromSummary == r.IsExcludedFromSummary &&
                db.Source == r.Source
            );
            if (exists) duplicateRecords.Add(r);
        }
        if (duplicateRecords.Count > 0)//若出现重复记录，则返回冲突状态码
        {
            return Conflict(new
            {
                message = "数据库中已存在重复记录，本次上传的所有数据未导入数据库。",
                duplicates = duplicateRecords.Select(d => new
                {
                    d.Date,
                    d.Value,
                    d.Owner,
                    d.ItemType,
                    d.IsVisitCount,
                    d.IsExcludedFromSummary,
                    d.Source
                })
            });
        }

        // 没有重复项，生成主键并保存
        foreach (var r in records)
            r.Id = Guid.NewGuid();

        try
        {
            await _context.RevenueRecords.AddRangeAsync(records);
            await _context.SaveChangesAsync();

            return Ok(new { message = "导入成功", count = records.Count });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "数据库更新失败", error = ex.Message });
        }
        catch (Exception ex)
        {
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
    [HttpGet("home/summary/monthly")]
    public async Task<ActionResult<MonthlySummaryDto>> GetMonthlySummary(DateOnly date)
    {
        var targetMonthStart = new DateOnly(date.Year, date.Month, 1);  // 获取目标月的第一天
        var targetMonthEnd = targetMonthStart.AddMonths(1);  // 终点为即下月1号，用于 "<" 判断

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

        int activeDays = recordsInMonth//当前月有数据的日数
            .Select(r => r.Date)
            .Distinct()
            .Count();

        //日均数据
        decimal averageDaily = activeDays > 0 ? totalMonthly / activeDays : 0;

        //包含当月所有每日数据
        var dailyMap = recordsInMonth
            .GroupBy(r => r.Date)
            .ToDictionary(g => g.Key, g => g.Sum(AdjustedValue));

        var result = new MonthlySummaryDto
        {
            TotalMonthly = totalMonthly,
            AverageDaily = Math.Round(averageDaily, 2),
            DailyMap = dailyMap
        };

        return Ok(result);
    }

    /// <summary>
    /// 获取“当前月份”各医生营收汇总 + 本月营业天数。
    /// 退费按负值扣减；TotalVisits 为 IsVisitCount == true 的 Value 累加。
    /// </summary>
    [HttpGet("doctors/summary")]
    public async Task<ActionResult<DoctorMonthlySummaryDto>> GetDoctorsSummaryByMonth(
        [FromQuery] int year,
        [FromQuery] int month)
    {
        if (month < 1 || month > 12) return BadRequest("month 必须为 1-12");

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
                                .Sum(r => r.ItemType == "退费" ? -r.Value : r.Value),
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
