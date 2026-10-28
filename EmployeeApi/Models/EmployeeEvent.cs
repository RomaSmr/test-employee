namespace EmployeeApi.Models
{
    public enum EventType
    {
        Hire = 1,
        Transfer = 2, 
        Fire = 3
    }

    public class EmployeeEvent
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public EventType Type { get; set; }
        public DateOnly EventDate { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public string Position { get; set; }
    }
}