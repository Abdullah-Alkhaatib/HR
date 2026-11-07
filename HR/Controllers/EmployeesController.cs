using Microsoft.AspNetCore.Mvc;
using HR.Model;
using HR.DTOs.Employees;
using HR.DTOs.Shared;
using HR.Enums;
using Microsoft.AspNetCore.Authorization;

namespace HR.Controllers
{
    [Authorize]
    [Route("api/Employees")] // Data annotation => يعني انو كل ال API routes رح تبدا ب api/Employees
    [ApiController] // Data annotation
    public class EmployeesController : ControllerBase
    {
        private HrDbContext _dbContext;
        private IWebHostEnvironment _env;
        private readonly IConfiguration _config; 

        // NuGet Package => Library
        public EmployeesController(HrDbContext dbContext, IWebHostEnvironment env, IConfiguration config    )
        {
            _dbContext = dbContext;
            _env = env;
            _config = config;
        }

        //[Authorize(Roles = "HR, Admin")]
        [HttpGet("GetAll")] // Data annotation => يعني انو هذا ال method رح يكون GET request]
        public IActionResult GetAll([FromQuery] FilterEmployeeDto filterDto) // Query parameter, Position is optional parameter
        {
            try
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
                           orderby employee.Id descending
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
                               DepartmentName = department.Name,
                               ImagePath = employee.ImagePath != null ? Path.Combine(_config["BaseUrl"], employee.ImagePath) : ""
                           };
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetById")]
        public IActionResult GetById([FromQuery] long id)
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Add")]
        public IActionResult Add([FromForm] SaveEmployeeDto employeeDto)
        {
            try
            {
                var user = new User()
                {
                    Id = 0, // Ignored
                    Username = $"{employeeDto.Name}_HR",
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword($"{employeeDto.Name}@123"), // يعني رح اعمل hash لل password قبل ما احفظو بال database
                    IsAdmin = false
                };

                var _user = _dbContext.Users.FirstOrDefault(x => x.Username.ToUpper() == user.Username.ToUpper());
                if(_user != null)
                {
                    return BadRequest("Cannot Add This Emplpyee : The Username Already Exists Please Select Another Name.");
                };

                _dbContext.Add(user);

                var employee = new Employee()
                {
                    //Id = employees.Max(x => x.Id) + 1, // Auto increment Id
                    //Id = (employees.LastOrDefault()?.Id ?? 0) + 1, // Auto increment Id
                    Id = 0, // Ignored
                    Name = employeeDto.Name,
                    //Age = employeeDto.Age,
                    BirthDate = employeeDto.BirthDate,
                    Phone = employeeDto.Phone,
                    //Position = employeeDto.Position,
                    PositionId = employeeDto.PositionId,
                    IsActive = employeeDto.IsActive,
                    StartDate = employeeDto.StartDate,
                    EndDate = employeeDto.EndDate,
                    DepartmentId = employeeDto.DepartmentId,
                    ManagerId = employeeDto.ManagerId,
                    User = user,
                    ImagePath = null
                };

                if(employeeDto.Image != null)
                {
                    employee.ImagePath = UploadImage(employeeDto.Image);
                }

                _dbContext.Employees.Add(employee);

                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public IActionResult Update([FromForm] SaveEmployeeDto employeeDto)
        {
            try
            {
                var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == employeeDto.Id);
                if (employee == null)
                {
                    return BadRequest("Employee not found."); // 400
                }
                employee.Name = employeeDto.Name;
                //employee.Age = employeeDto.Age;
                employee.BirthDate = employeeDto.BirthDate;
                //employee.Position = employeeDto.Position;
                employee.PositionId = employeeDto.PositionId;
                employee.IsActive = employeeDto.IsActive;
                employee.Phone = employeeDto.Phone;
                employee.StartDate = employeeDto.StartDate;
                employee.EndDate = employeeDto.EndDate;
                employee.DepartmentId = employeeDto.DepartmentId;
                employee.ManagerId = employeeDto.ManagerId;

                if(employeeDto.Image != null)
                {
                    employee.ImagePath = UploadImage(employeeDto.Image);
                }
                else if (employeeDto.IsImage == false && employeeDto.Image == null)
                {
                    employee.ImagePath = null;
                }

                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public IActionResult Delete([FromQuery] long id)
        {
            try
            {
                var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == id);
                if (employee == null)
                {
                    return BadRequest("Employee not found."); // 400
                }

                var employeedAssociate = _dbContext.Employees.FirstOrDefault(x => x.ManagerId == id); // Employee is manager
                if (employeedAssociate != null)
                {
                    return BadRequest("Managers with assigned employees cannot be deleted.");
                }

                _dbContext.Employees.Remove(employee);
                _dbContext.SaveChanges();

                return Ok($"Employee with Id {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get managers
        [HttpGet("GetManagersList")]
        public IActionResult GetManagers([FromQuery] long? employeeId)
        {
            var data = from emp in _dbContext.Employees
                       from pos in _dbContext.Lookups.Where(x => x.Id == emp.PositionId)
                       where emp.IsActive &&
                             emp.Id != employeeId &&
                             pos.MajorCode == (int)LookupMajorCodes.EmployeePosition &&
                             pos.MinorCode == (int)PositionsMinorCodes.Manager
                       select new ListDto
                       {
                           Id = emp.Id,
                           Name = emp.Name
                       };

            return Ok(data);
        }


        private string UploadImage(IFormFile image)
        {
            //var folderPath = "C:\\Users\\abdul\\source\\repos\\HR\\Attachments\\EmployeeImages";
            var imagesFolderPath = Path.Combine( "Attachments", "EmployeeImages");

            var folderPath = Path.Combine( _env.WebRootPath, imagesFolderPath);
            if (!Directory.Exists(folderPath))
            {
                // Create Folder
                Directory.CreateDirectory(folderPath);
            }

            var fileExtension = Path.GetExtension(image.FileName); // .png .jpg .jpeg ...
            var fileName = Guid.NewGuid() + fileExtension; // Generate unique file name => 1234-5678-90ab-cdefm + .png

            var filePath = Path.Combine(folderPath, fileName);

            var stream = new FileStream(filePath, FileMode.Create); // Create file
            using (stream)
            {
                image.CopyTo(stream); // Copy image to file Steam
            }

            return Path.Combine(imagesFolderPath, fileName);
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