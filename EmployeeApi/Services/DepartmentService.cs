using EmployeeApi.Data;
using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _db;
        private readonly IValidator<Department> _validator;

        public DepartmentService(AppDbContext db, IValidator<Department> validator)
        {
            _db = db;
            _validator = validator;
        }

        public async Task<List<DepartmentItem>> GetAll()
        {
            return await _db.Departments
                .OrderBy(d => d.Code)
                .Select(d => new DepartmentItem
                {
                    Id = d.Id,
                    Code = d.Code,
                    FullName = d.FullName,
                    Abbreviation = d.Abbreviation,
                    CreatedDate = d.CreatedDate,
                    ClosedDate = d.ClosedDate,
                    ParentId = d.ParentId,
                    ParentName = d.Parent.FullName
                })
                .ToListAsync();
        }

        public async Task<Result> Create(Department dep)
        {
            var check = await _validator.ValidateAsync(dep);
            if (!check.IsValid)
                return Result.Fail(check.Errors.First().ErrorMessage);

            if (await _db.Departments.AnyAsync(d => d.Code == dep.Code))
                return Result.Fail("Такой код уже есть");

            _db.Departments.Add(dep);
            await _db.SaveChangesAsync();
            return Result.Ok(dep.Id);
        }

        public async Task<Result> Edit(int id, Department dep)
        {
            var old = await _db.Departments.FindAsync(id);
            if (old == null)
                return Result.NotFoundResult();

            var check = await _validator.ValidateAsync(dep);
            if (!check.IsValid)
                return Result.Fail(check.Errors.First().ErrorMessage);

            if (await _db.Departments.AnyAsync(d => d.Code == dep.Code && d.Id != id))
                return Result.Fail("Такой код уже есть");

            if (dep.ParentId == id)
                return Result.Fail("Подразделение не может быть родителем самого себя");

            old.Code = dep.Code;
            old.FullName = dep.FullName;
            old.Abbreviation = dep.Abbreviation;
            old.CreatedDate = dep.CreatedDate;
            old.ClosedDate = dep.ClosedDate;
            old.ParentId = dep.ParentId;

            await _db.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> Close(int id)
        {
            var dep = await _db.Departments.FindAsync(id);
            if (dep == null)
                return Result.NotFoundResult();

            dep.ClosedDate = DateOnly.FromDateTime(DateTime.Today);
            await _db.SaveChangesAsync();
            return Result.Ok();
        }
    }
}