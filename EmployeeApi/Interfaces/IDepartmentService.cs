using EmployeeApi.Models;

namespace EmployeeApi.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentItem>> GetAll();
        Task<Result> Create(Department dep);
        Task<Result> Edit(int id, Department dep);
        Task<Result> Close(int id);
    }
}