using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HR.Model;
using HR.DTOs.Employees;
using HR.DTOs.Employees;

namespace HR.Controllers
{
    [Route("api/Employees")] // Data annotation => يعني انو كل ال API routes رح تبدا ب api/Employees
    [ApiController] // Data annotation
    public class EmployeesController : ControllerBase
    {
        private HrDbContext _dbContext;

        // NuGet Package => Library
        public EmployeesController(HrDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("GetAll")] // Data annotation => يعني انو هذا ال method رح يكون GET request]
        public IActionResult GetAll([FromQuery]FilterEmployeeDto filterDto) // Query parameter, Position is optional parameter
        {
            // LINQ query syntax
            var data = from employee in _dbContext.Employees
                       from department in _dbContext.Departments.Where(x => x.Id == employee.DepartmentId).DefaultIfEmpty() // Left Join
                       from manager in _dbContext.Employees.Where(x => x.Id == employee.ManagerId).DefaultIfEmpty() // Left Join
                       from lookup in _dbContext.Lookups.Where(x => x.Id == employee.PositionId).DefaultIfEmpty() // Left Join
                                                                                                                   //where employee.Position == position
                                                                                                                   //where (position == null || employee.Position == position) // if position is null, return all employees
                       where 
                       (filterDto.PositionId == null || employee.PositionId == filterDto.PositionId) && 
                             (filterDto.EmployeeName == null || employee.Name.ToUpper().Contains(filterDto.EmployeeName.ToUpper())) &&
                             (filterDto.IsActive == null || employee.IsActive == filterDto.IsActive)
                       //orderby employee.Age
                       orderby employee.Id
                       select new EmployeeDto
                       {
                           Id = employee.Id,
                           Name = employee.Name,
                           //Age = employee.Age,
                           BirthDate = employee.BirthDate,
                           //Position = employee.Position,
                           PositionId = employee.PositionId,
                           PositionName = lookup.Name,
                           IsActive = employee.IsActive,
                           StartDate = employee.StartDate,
                           Phone = employee.Phone,
                           ManagerId = employee.ManagerId,
                           ManagerName = manager.Name,
                           DepartmentId = employee.DepartmentId,
                           DepartmentName = department.Name
                       };
            return Ok(data);
        }

        [HttpGet("GetById")]
        public IActionResult GetById([FromQuery]long id)
        {
            // عشان يرجعلي فقط object بدون []
            var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == id); // firstOrDefault => بتجيب اول object او null اذا ما لقتو
            if (employee == null)
            {
                return NotFound($"Employee with Id {id} not found.");
            }
            var employeeDto = new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                //Age = employee.Age,
                BirthDate = employee.BirthDate,
                //Position = employee.Position,
                PositionId = employee.PositionId,
                PositionName = employee.Lookup.Name,
                IsActive = employee.IsActive,
                StartDate = employee.StartDate,
                Phone = employee.Phone,
                ManagerId = employee.ManagerId,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.DepartmentRow.Name,
                ManagerName = employee.Manager.Name
            };

            return Ok(employeeDto);
        }

        [HttpPost("Add")]
        public IActionResult Add([FromBody] SaveEmployeeDto employeeDto)
        {
            var employee = new Employee()
            {
                //Id = employees.Max(x => x.Id) + 1, // Auto increment Id
                //Id = (employees.LastOrDefault()?.Id ?? 0) + 1, // Auto increment Id
                Id = 0, // Ignored
                Name = employeeDto.Name,
                //Age = employeeDto.Age,
                BirthDate = employeeDto.BirthDate,
                //Position = employeeDto.Position,
                PositionId = employeeDto.PositionId,
                IsActive = employeeDto.IsActive,
                StartDate = employeeDto.StartDate,
                EndDate = employeeDto.EndDate,
                DepartmentId = employeeDto.DepartmentId,
                ManagerId = employeeDto.ManagerId
            };

            _dbContext.Employees.Add(employee);
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] SaveEmployeeDto employeeDto)
        {
            var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == employeeDto.Id);
            if(employee == null)
            {
                return BadRequest("Employee not found."); // 400
            }
            employee.Name = employeeDto.Name;
            //employee.Age = employeeDto.Age;
            employee.BirthDate = employeeDto.BirthDate;
            //employee.Position = employeeDto.Position;
            employee.PositionId = employeeDto.PositionId;
            employee.IsActive = employeeDto.IsActive;
            employee.StartDate = employeeDto.StartDate;
            employee.EndDate = employeeDto.EndDate;
            employee.DepartmentId = employeeDto.DepartmentId;
            employee.ManagerId = employeeDto.ManagerId;

            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("Delete")]
        public IActionResult Delete([FromQuery]long id)
        {
            var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
            {
                return BadRequest("Employee not found."); // 400
            }
            _dbContext.Employees.Remove(employee);
            _dbContext.SaveChanges();

            return Ok($"Employee with Id {id} deleted successfully.");
        }
    }
}


// Simple data type : long, int , string, ... | Query Parameter(By Default)
// Complex data type : Model, DTo (object) | Request Body(By Default)

// Http Get : Can not use body request [FromBody], We can use query parameter [FromQuery]
// Tttp Put, Post : Can use both body request [FromBody] and query parameter [FromQuery], But we will only use [FromBody]
// Http Delete : Can use both body request [FromBody] and query parameter [FromQuery], But we will only use [FromQuery]

// Can't use multiple parameters of type [FromBody]
// Can use multiple parameters of type [FromQuery]