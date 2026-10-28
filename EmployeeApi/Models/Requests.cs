namespace EmployeeApi.Models
{
    public class TransferRequest
    {
        public int DepartmentId { get; set; }
        public string Position { get; set; }
        public DateOnly Date { get; set; }
    }

    public class FireRequest
    {
        public DateOnly Date { get; set; }
    }
}