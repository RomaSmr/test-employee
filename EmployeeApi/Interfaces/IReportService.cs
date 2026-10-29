using EmployeeApi.Models;

namespace EmployeeApi.Interfaces
{
    public interface IReportService
    {
        Task<Result<List<ReportItem>>> Build(ReportFilter filter);
    }
}