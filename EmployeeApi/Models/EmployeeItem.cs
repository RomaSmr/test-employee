namespace EmployeeApi.Models
{
    public class EmployeeItem
    {
        public int Id { get; set; }
        public string TabNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public Gender Gender { get; set; }
        public string Position { get; set; }
        public string PhotoPath { get; set; }
        public DateOnly? FireDate { get; set; }
        public string DepartmentName { get; set; }
    }
}