using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using EmployeeApi.Services;
using EmployeeApi.Validators;
using FluentValidation;

namespace EmployeeApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IValidator<Department>, DepartmentValidator>();
            services.AddScoped<IValidator<Employee>, EmployeeValidator>();
            services.AddScoped<IValidator<TransferRequest>, TransferValidator>();
            services.AddScoped<IValidator<ReportFilter>, ReportFilterValidator>();

            return services;
        }
    }
}