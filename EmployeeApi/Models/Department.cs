namespace EmployeeApi.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string FullName {get; set; }
        public string Abbreviation { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly? ClosedDate { get; set; }

        public int? ParentId { get; set; }
        public Department Parent { get; set;}
        public List<Department> Children {get; set; } = new List<Department>();
    }
}