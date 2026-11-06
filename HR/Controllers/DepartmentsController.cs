using HR.DTOs.Departments;
using HR.DTOs.Shared;
using HR.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Controllers
{
    //[Authorize]
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
            try
            {
                var data = from department in _dbContext.Departments
                           from lookup in _dbContext.Lookups.Where(x => x.Id == department.TypeId).DefaultIfEmpty() // Left Join
                           where (filterDepartmentDto.DepartmentName == null || department.Name.ToUpper().Contains(filterDepartmentDto.DepartmentName.ToUpper())) &&
                           (filterDepartmentDto.FloorNumber == null || department.FloorNumber == filterDepartmentDto.FloorNumber)
                           select new DepartmentDto
                           {
                               Id = department.Id,
                               Name = department.Name,
                               Description = department.Description,
                               FloorNumber = department.FloorNumber,
                               TypeId = lookup.Id,
                               TypeName = lookup.Name
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
                var department = _dbContext.Departments.Select(x => new DepartmentDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    FloorNumber = x.FloorNumber,
                    TypeId = x.Lookup.Id,
                    TypeName = x.Lookup.Name
                }).FirstOrDefault(x => x.Id == id); // FirstOrDefault=> اذا بدي ارجع اشي وما لقيت ارجع null, where => اذا بدي ارجع اكثر من اشي

                return Ok(department);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Add")]
        public IActionResult Add([FromBody] SaveDepartmentDto departmentDto)
        {
            try
            {
                var department = new Department
                {
                    Id = 0,
                    Name = departmentDto.Name,
                    Description = departmentDto.Description,
                    FloorNumber = departmentDto.FloorNumber,
                    TypeId = departmentDto.TypeId
                };
                _dbContext.Departments.Add(department);
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] SaveDepartmentDto departmentDto)
        {
            try
            {
                var department = _dbContext.Departments.FirstOrDefault(x => x.Id == departmentDto.Id); 
                if (department == null)
                {
                    return BadRequest("Department does not exist");
                }

                department.Name = departmentDto.Name;
                department.Description = departmentDto.Description;
                department.FloorNumber = departmentDto.FloorNumber;
                department.TypeId = departmentDto.TypeId;

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
                var department = _dbContext.Departments.FirstOrDefault(x => x.Id == id);
                if (department == null)
                {
                    return NotFound("Department does not exist");
                }
                _dbContext.Departments.Remove(department);
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDepartmentsList")]
        public IActionResult GetDepartmentsList()
        {
            try
            {
                var departments = _dbContext.Departments.Select(x => new ListDto
                {
                    Id = x.Id,
                    Name = x.Name
                });

                return Ok(departments);

            } 
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
