using EmployeeApi.Data;
using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _db;
        private readonly IValidator<ReportFilter> _validator;

        public ReportService(AppDbContext db, IValidator<ReportFilter> validator)
        {
            _db = db;
            _validator = validator;
        }

        public async Task<Result<List<ReportItem>>> Build(ReportFilter filter)
        {
            var check = await _validator.ValidateAsync(filter);
            if (!check.IsValid)
                return Result<List<ReportItem>>.Fail(check.Errors.First().ErrorMessage);

            var query = _db.EmployeeEvents
                .Where(e => e.EventDate >= filter.From && e.EventDate <= filter.To);

            if (filter.Type != null)
                query = query.Where(e => e.Type == filter.Type);

            var list = await query
                .OrderBy(e => e.EventDate)
                .ThenBy(e => e.Employee.LastName)
                .Select(e => new ReportItem
                {
                    Id = e.Id,
                    EventDate = e.EventDate,
                    Type = e.Type,
                    TabNumber = e.Employee.TabNumber,
                    LastName = e.Employee.LastName,
                    FirstName = e.Employee.FirstName,
                    MiddleName = e.Employee.MiddleName,
                    DepartmentName = e.Department.FullName,
                    Position = e.Position
                })
                .ToListAsync();

            return Result<List<ReportItem>>.Ok(list);
        }
    }
}