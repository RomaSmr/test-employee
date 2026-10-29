namespace EmployeeApi.Models
{
    public class EmployeeDetails
    {
        public int Id { get; set; }
        public string TabNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PhotoPath { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Position { get; set; }
        public DateOnly HireDate { get; set; }
        public DateOnly? FireDate { get; set; }
        public int Age { get; set; }
        public int Experience { get; set; }
    }
}