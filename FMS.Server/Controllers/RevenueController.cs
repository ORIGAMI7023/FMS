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

    public RevenueController(AppDbContext context) { _context = context; }

    [HttpPost("import")]
    public async Task<IActionResult> ImportFromJson([FromBody] List<RevenueRecord> records)
    {
        Console.WriteLine("🔥 收到 POST /import 请求，共有记录数：" + records?.Count);

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
}
