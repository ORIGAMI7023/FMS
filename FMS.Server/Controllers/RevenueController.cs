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



}
