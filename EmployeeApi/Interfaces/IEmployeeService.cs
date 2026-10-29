using EmployeeApi.Models;

namespace EmployeeApi.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeItem>> GetAll();
        Task<EmployeeDetails> GetOne(int id);
        Task<Result> Hire(Employee emp);
        Task<Result> Edit(int id, Employee emp);
        Task<Result> Transfer(int id, TransferRequest request);
        Task<Result> Fire(int id, FireRequest request);
        Task<Result<string>> UploadPhoto(int id, IFormFile file);
    }
}