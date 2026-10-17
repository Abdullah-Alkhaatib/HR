using HR.DTOs.Departments;
using HR.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        //public static List<Department> departments = new List<Department>()
        //{
        //    new Department { Id = 0, Name = "Department1", Description = "Employees Department", FloorNumber = 1 },
        //    new Department { Id = 1, Name = "Department2", Description = "Employees Department", FloorNumber = 2 },
        //    new Department { Id = 2, Name = "Department3", Description = "Employees Department", FloorNumber = 3 },
        //};
        private readonly HrDbContext _dbContext;

        public DepartmentsController(HrDbContext dbContext)
        {
            this._dbContext = dbContext;

        }

        [HttpGet("GetAll")]
        public IActionResult GetAll([FromQuery] FilterDepartmentsDto filterDepartmentDto)
        {
            var data = from department in _dbContext.Departments
                       where (filterDepartmentDto.DepartmentName == null || department.Name.ToUpper().Contains(filterDepartmentDto.DepartmentName.ToUpper())) &&
                       (filterDepartmentDto.FloorNumber == null || department.FloorNumber == filterDepartmentDto.FloorNumber)
                       select new DepartmentDto
                       {
                           Id = department.Id,
                           Name = department.Name,
                           Description = department.Description,
                           FloorNumber = department.FloorNumber
                       };
            return Ok(data);
        }

        [HttpGet("GetById")]
        public IActionResult GetById([FromQuery] long id)
        {
            var department = _dbContext.Departments.FirstOrDefault(x => x.Id == id);
            if (department == null)
            {
                return NotFound("Department does not exist");
            }
            var departmentDto = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                FloorNumber = department.FloorNumber
            };

            return Ok(departmentDto);
        }

        [HttpPost("Add")]
        public IActionResult Add([FromBody] SaveDepartmentDto departmentDto)
        {
            var department = new Department
            {
                Id = 0,
                Name = departmentDto.Name,
                Description = departmentDto.Description,
                FloorNumber = departmentDto.FloorNumber
            };
            _dbContext.Departments.Add(department);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] SaveDepartmentDto departmentDto)
        {
            var department = _dbContext.Departments.FirstOrDefault(x => x.Id == departmentDto.Id);
            if (department == null)
            {
                return NotFound("Department does not exist");
            }
            department.Name = departmentDto.Name;
            department.Description = departmentDto.Description;
            department.FloorNumber = departmentDto.FloorNumber;

            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("Delete")]
        public IActionResult Delete([FromQuery] long id)
        {
            var department = _dbContext.Departments.FirstOrDefault(x => x.Id == id);
            if (department == null)
            {
                return NotFound("Department does not exist");
            }
            _dbContext.Departments.Remove(department);
            _dbContext.SaveChanges();

            return Ok();
        }
    }
}
