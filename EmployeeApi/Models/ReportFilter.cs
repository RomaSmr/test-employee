namespace EmployeeApi.Models
{
    public class ReportFilter
    {
        public DateOnly? From { get; set; }
        public DateOnly? To { get; set; }
        public EventType? Type { get; set;}
    }
}