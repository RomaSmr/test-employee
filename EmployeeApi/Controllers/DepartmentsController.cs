using EmployeeApi.Data;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/departments")]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DepartmentsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task <IActionResult> GetAll()
        {
            var list = await _db.Departments
                .OrderBy(d => d.Code)
                .Select(d => new
                {
                    d.Id,
                    d.Code,
                    d.FullName,
                    d.Abbreviation,
                    d.CreatedDate,
                    d.ClosedDate,
                    d.ParentId,
                    ParentName = d.Parent.FullName
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Department dep)
        {
            if (dep.Code == null || dep.Code.Length != 4)
                return BadRequest("Код должен быть из 4 символов, больше нельзя я так сказал :)");

            if (await _db.Departments.AnyAsync(d => d.Code == dep.Code))
                return BadRequest("Поздно, такой код уже используют");

            _db.Departments.Add(dep);
            await _db.SaveChangesAsync();
            return Ok(new {dep.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Department dep)
        {
            var old = await _db.Departments.FindAsync(id);
            if (old == null)
                return NotFound();

            if (dep.Code == null || dep.Code.Length != 4)
                return BadRequest("Код должен быть из 4 символов, больше нельзя я так сказал :)");

            if (await _db.Departments.AnyAsync(d => d.Code == dep.Code && d.Id != id))
                return BadRequest("Поздно, такой код уже используют");

            if (dep.ParentId == id)
                return BadRequest("Подразделение не может быть родителем самого себя");

            old.Code = dep.Code;
            old.FullName = dep.FullName;
            old.Abbreviation = dep.Abbreviation;
            old.CreatedDate = dep.CreatedDate;
            old.ClosedDate = dep.ClosedDate;
            old.ParentId = dep.ParentId;

            await _db.SaveChangesAsync();
            return Ok();  
        }

        [HttpPost("{id}/close")]
        public async Task<IActionResult> Close(int id)
        {
            var dep = await _db.Departments.FindAsync(id);
            if (dep == null)
                return NotFound();

            dep.ClosedDate = DateOnly.FromDateTime(DateTime.Today);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }

}