namespace HR.DTOs.Employees
{
    public class SaveEmployeeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        //public int? Age { get; set; }
        public DateTime? BirthDate { get; set; }
        //public string Position { get; set; }
        public long? PositionId { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? DepartmentId { get; set; }
        public long? ManagerId { get; set; }
    }
}
