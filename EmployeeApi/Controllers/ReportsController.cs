using EmployeeApi.Data;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReportsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DateOnly? from, DateOnly? to, EventType? type)
        {
            if (from == null || to == null)
                return BadRequest("Укажите период");

            if (from > to)
                return BadRequest("Начало периода позже конца, давай по нормальному");

            var query = _db.EmployeeEvents
                .Where(e => e.EventDate >= from && e.EventDate <= to);

            if (type != null)
                query = query.Where(e => e.Type == type);

            var list = await query
                .OrderBy(e => e.EventDate)
                .ThenBy(e => e.Employee.LastName)
                .Select(e => new
                {
                    e.Id,
                    e.EventDate,
                    e.Type,
                    e.Employee.TabNumber,
                    e.Employee.LastName,
                    e.Employee.FirstName,
                    e.Employee.MiddleName,
                    DepartmentName = e.Department.FullName,
                    e.Position
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}