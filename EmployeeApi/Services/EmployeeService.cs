using EmployeeApi.Data;
using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IValidator<Employee> _employeeValidator;
        private readonly IValidator<TransferRequest> _transferValidator;

        public EmployeeService(
            AppDbContext db,
            IWebHostEnvironment env,
            IValidator<Employee> employeeValidator,
            IValidator<TransferRequest> transferValidator)
        {
            _db = db;
            _env = env;
            _employeeValidator = employeeValidator;
            _transferValidator = transferValidator;
        }

        private static int FullYears(DateOnly from, DateOnly to)
        {
            int years = to.Year - from.Year;
            if (from.AddYears(years) > to)
                years--;
            return years;
        }

        public async Task<List<EmployeeItem>> GetAll()
        {
            return await _db.Employees
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Select(e => new EmployeeItem
                {
                    Id = e.Id,
                    TabNumber = e.TabNumber,
                    LastName = e.LastName,
                    FirstName = e.FirstName,
                    MiddleName = e.MiddleName,
                    Gender = e.Gender,
                    Position = e.Position,
                    PhotoPath = e.PhotoPath,
                    FireDate = e.FireDate,
                    DepartmentName = e.Department.FullName
                })
                .ToListAsync();
        }

        public async Task<EmployeeDetails> GetOne(int id)
        {
            var employee = await _db.Employees
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
                return null;

            var today = DateOnly.FromDateTime(DateTime.Today);

            var firstHire = await _db.EmployeeEvents
                .Where(x => x.EmployeeId == id && x.Type == EventType.Hire)
                .OrderBy(x => x.EventDate)
                .Select(x => (DateOnly?)x.EventDate)
                .FirstOrDefaultAsync();

            var start = firstHire ?? employee.HireDate;
            var end = employee.FireDate ?? today;

            return new EmployeeDetails
            {
                Id = employee.Id,
                TabNumber = employee.TabNumber,
                LastName = employee.LastName,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                Gender = employee.Gender,
                BirthDate = employee.BirthDate,
                Phone = employee.Phone,
                Email = employee.Email,
                PhotoPath = employee.PhotoPath,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department.FullName,
                Position = employee.Position,
                HireDate = employee.HireDate,
                FireDate = employee.FireDate,
                Age = FullYears(employee.BirthDate, today),
                Experience = FullYears(start, end)
            };
        }

        public async Task<Result> Hire(Employee emp)
        {
            var check = await _employeeValidator.ValidateAsync(emp);
            if (!check.IsValid)
                return Result.Fail(check.Errors.First().ErrorMessage);

            if (await _db.Employees.AnyAsync(x => x.TabNumber == emp.TabNumber))
                return Result.Fail("Сотрудник с таким табельным номером уже есть");

            var dep = await _db.Departments.FindAsync(emp.DepartmentId);
            if (dep == null)
                return Result.Fail("Подразделение не найдено");

            if (dep.ClosedDate != null)
                return Result.Fail("Подразделение ликвидировано, принимать в него нельзя");

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

            return Result.Ok(emp.Id);
        }

        public async Task<Result> Edit(int id, Employee emp)
        {
            var oldEmployee = await _db.Employees.FindAsync(id);
            if (oldEmployee == null)
                return Result.NotFoundResult();

            var check = await _employeeValidator.ValidateAsync(emp);
            if (!check.IsValid)
                return Result.Fail(check.Errors.First().ErrorMessage);

            if (await _db.Employees.AnyAsync(x => x.TabNumber == emp.TabNumber && x.Id != id))
                return Result.Fail("Сотрудник с таким табельным номером уже есть");

            oldEmployee.TabNumber = emp.TabNumber;
            oldEmployee.LastName = emp.LastName;
            oldEmployee.FirstName = emp.FirstName;
            oldEmployee.MiddleName = emp.MiddleName;
            oldEmployee.Gender = emp.Gender;
            oldEmployee.BirthDate = emp.BirthDate;
            oldEmployee.Phone = emp.Phone;
            oldEmployee.Email = emp.Email;

            await _db.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> Transfer(int id, TransferRequest request)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null)
                return Result.NotFoundResult();

            var check = await _transferValidator.ValidateAsync(request);
            if (!check.IsValid)
                return Result.Fail(check.Errors.First().ErrorMessage);

            if (emp.FireDate != null)
                return Result.Fail("Сотрудник уже уволен");

            var dep = await _db.Departments.FindAsync(request.DepartmentId);
            if (dep == null)
                return Result.Fail("Подразделение не найдено");

            if (dep.ClosedDate != null)
                return Result.Fail("Подразделение ликвидировано");

            if (request.DepartmentId == emp.DepartmentId && request.Position == emp.Position)
                return Result.Fail("Подразделение и должность не изменились");

            if (request.Date < emp.HireDate)
                return Result.Fail("Дата перевода раньше даты прибытия, так нельзя");

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
            return Result.Ok();
        }

        public async Task<Result> Fire(int id, FireRequest request)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null)
                return Result.NotFoundResult();

            if (emp.FireDate != null)
                return Result.Fail("Сотрудник уже уволен");

            if (request.Date < emp.HireDate)
                return Result.Fail("Дата увольнения раньше даты приёма, это как так у вас получилось?");

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
            return Result.Ok();
        }

        public async Task<Result<string>> UploadPhoto(int id, IFormFile file)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null)
                return Result<string>.NotFoundResult();

            if (file == null || file.Length == 0)
                return Result<string>.Fail("Файл не выбран");

            if (file.Length > 2 * 1024 * 1024)
                return Result<string>.Fail("Файл больше 2 МБ");

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                return Result<string>.Fail("Только jpg или png");

            if (_env.WebRootPath == null)
                return Result<string>.Fail("На сервере нет папки wwwroot");

            var folder = Path.Combine(_env.WebRootPath, "photos");
            Directory.CreateDirectory(folder);

            if (!string.IsNullOrEmpty(emp.PhotoPath))
            {
                var oldFile = Path.Combine(_env.WebRootPath, emp.PhotoPath.TrimStart('/'));
                if (File.Exists(oldFile))
                    File.Delete(oldFile);
            }

            var fileName = Guid.NewGuid() + ext;
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            emp.PhotoPath = "/photos/" + fileName;
            await _db.SaveChangesAsync();

            return Result<string>.Ok(emp.PhotoPath);
        }
    }
}