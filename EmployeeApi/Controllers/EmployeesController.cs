using EmployeeApi.Data;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public EmployeesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private int GetYears(DateOnly from, DateOnly to)
        {
            int days = to.DayNumber - from.DayNumber;
            int years = days / 365;
            return years;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Employees
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Select(e => new
                {
                    e.Id,
                    e.TabNumber,
                    e.LastName,
                    e.FirstName,
                    e.MiddleName,
                    e.Gender,
                    e.Position,
                    e.PhotoPath,
                    e.FireDate,
                    DepartmentName = e.Department.FullName
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var employee = await _db.Employees
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
                return NotFound();

            var today = DateOnly.FromDateTime(DateTime.Today);

            var firstHire = await _db.EmployeeEvents
                .Where(x => x.EmployeeId == id && x.Type == EventType.Hire)
                .OrderBy(x => x.EventDate)
                .Select(x => x.EventDate)
                .FirstAsync();

            var end = employee.FireDate ?? today;

            return Ok(new
            {
                employee.Id,
                employee.TabNumber,
                employee.LastName,
                employee.FirstName,
                employee.MiddleName,
                employee.Gender,
                employee.BirthDate,
                employee.Phone,
                employee.Email,
                employee.PhotoPath,
                employee.DepartmentId,
                DepartmentName = employee.Department.FullName,
                employee.Position,
                employee.HireDate,
                employee.FireDate,
                Age = GetYears(employee.BirthDate, today),
                Experience = GetYears(firstHire, end)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Hire([FromBody] Employee emp)
        {
            if (string.IsNullOrWhiteSpace(emp.TabNumber))
                return BadRequest("Укажите таб. номер");

            if (string.IsNullOrWhiteSpace(emp.LastName)
                || string.IsNullOrWhiteSpace(emp.FirstName)
                || string.IsNullOrWhiteSpace(emp.MiddleName))
                return BadRequest("Фамилия, имя и отчество обязательны");

            if (await _db.Employees.AnyAsync(x => x.TabNumber == emp.TabNumber))
                return BadRequest("Сотрудник с таким табельным номером уже есть");

            var dep = await _db.Departments.FindAsync(emp.DepartmentId);
            if (dep == null)
                return BadRequest("Подразделение ещё нет, но скоро будет");

            if (dep.ClosedDate != null)
                return BadRequest("Подразделение ликвидировано, принимать в него нельзя");

            emp.FireDate = null;
            emp.PhotoPath = null;

            var hireEvent = new EmployeeEvent
            {
                Employee = emp,
                Type = EventType.Hire,
                EventDate = emp.HireDate,
                DepartmentId = emp.DepartmentId,
                Position = emp.Position
            };

            _db.Employees.Add(emp);
            _db.EmployeeEvents.Add(hireEvent);
            await _db.SaveChangesAsync();

            return Ok(new { emp.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Employee emp)
        {
            var oldEmployee = await _db.Employees.FindAsync(id);
            if (oldEmployee == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(emp.LastName)
                || string.IsNullOrWhiteSpace(emp.FirstName)
                || string.IsNullOrWhiteSpace(emp.MiddleName))
                return BadRequest("Фамилия, имя и отчество обязательны");

            if (await _db.Employees.AnyAsync(x => x.TabNumber == emp.TabNumber && x.Id != id))
                return BadRequest("Сотрудник с таким табельным номером уже устроен на работу");

            oldEmployee.TabNumber = emp.TabNumber;
            oldEmployee.LastName = emp.LastName;
            oldEmployee.FirstName = emp.FirstName;
            oldEmployee.MiddleName = emp.MiddleName;
            oldEmployee.Gender = emp.Gender;
            oldEmployee.BirthDate = emp.BirthDate;
            oldEmployee.Phone = emp.Phone;
            oldEmployee.Email = emp.Email;

            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/transfer")]
        public async Task<IActionResult> Transfer(int id, [FromBody] TransferRequest request)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null)
                return NotFound();

            if (emp.FireDate != null)
                return BadRequest("Сотрудник уже кикнут");

            var dep = await _db.Departments.FindAsync(request.DepartmentId);
            if (dep == null)
                return BadRequest("Подразделение ещё нет, но скоро будет");

            if (dep.ClosedDate != null)
                return BadRequest("Подразделение ликвидировано, принимать в него нельзя");

            if (request.DepartmentId == emp.DepartmentId && request.Position == emp.Position)
                return BadRequest("Подразделение и должность остались те же");

            if (request.Date < emp.HireDate)
                return BadRequest("Дата перевода раньше даты прибытия");

            emp.DepartmentId = request.DepartmentId;
            emp.Position = request.Position;
            emp.HireDate = request.Date;

            _db.EmployeeEvents.Add(new EmployeeEvent
            {
                EmployeeId = emp.Id,
                Type = EventType.Transfer,
                EventDate = request.Date,
                DepartmentId = request.DepartmentId,
                Position = request.Position
            });

            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/fire")]
        public async Task<IActionResult> Fire(int id, [FromBody] FireRequest request)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null)
                return NotFound();

            if (emp.FireDate != null)
                return BadRequest("Сотрудник уже кикнут");

            if (request.Date < emp.HireDate)
                return BadRequest("Дата увольнения раньше даты приёма, такого быть не может");

            emp.FireDate = request.Date;

            _db.EmployeeEvents.Add(new EmployeeEvent
            {
                EmployeeId = emp.Id,
                Type = EventType.Fire,
                EventDate = request.Date,
                DepartmentId = emp.DepartmentId,
                Position = emp.Position
            });

            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/photo")]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null)
                return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран");

            if (file.Length > 2 * 1024 * 1024)
                return BadRequest("Файл больше 2 МБ");

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                return BadRequest("Только jpg или png");

            if (_env.WebRootPath == null)
                return BadRequest("На сервере нет папки wwwroot");

            var folder = Path.Combine(_env.WebRootPath, "photos");
            Directory.CreateDirectory(folder);

            if (!string.IsNullOrEmpty(emp.PhotoPath))
            {
                var oldFile = Path.Combine(_env.WebRootPath, emp.PhotoPath.TrimStart('/'));
                if (System.IO.File.Exists(oldFile))
                    System.IO.File.Delete(oldFile);
            }

            var fileName = Guid.NewGuid() + ext;
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            emp.PhotoPath = "/photos/" + fileName;
            await _db.SaveChangesAsync();

            return Ok(new { emp.Id, emp.PhotoPath });
        }
    }
}