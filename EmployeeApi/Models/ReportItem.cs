namespace EmployeeApi.Models
{
    public class ReportItem
    {
        public int Id { get; set; }
        public DateOnly EventDate { get; set; }
        public EventType Type { get; set; }
        public string TabNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string DepartmentName { get; set; }
        public string Position { get; set; }
    }
}