using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FMS.Server.Data;
using FMS.Server.Models;

namespace FMS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevenueController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RevenueController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.RevenueRecords.ToList());

        [HttpPost]
        public IActionResult Create([FromBody] RevenueRecord record)
        {
            record.Id = Guid.NewGuid();
            _context.RevenueRecords.Add(record);
            _context.SaveChanges();
            return Ok(record);
        }
    }
}
