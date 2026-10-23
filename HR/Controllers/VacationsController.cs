using HR.DTOs.Vacations;
using HR.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VacationsController : ControllerBase
    {
        private readonly HrDbContext _dbContext;

        public VacationsController(HrDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll([FromQuery] VacationsFilterDto vationsFilterDto)
        {
            var data = from vac in _dbContext.Vacations
                       from emp in _dbContext.Employees.Where(x => x.Id == vac.EmployeeId)
                       from lookup in _dbContext.Lookups.Where(x => x.Id == vac.TypeId)
                       where (vationsFilterDto.VacationTypeId == null || vationsFilterDto.VacationTypeId == vac.TypeId) // يعني إذا كان نوع الإجازة المحدد في الفلتر غير موجود، سيتم إرجاع جميع الإجازات بغض النظر عن نوعها
                       where (vationsFilterDto.EmployeeId == null || vationsFilterDto.EmployeeId == vac.EmployeeId) // يعني إذا كان معرف الموظف المحدد في الفلتر غير موجود، سيتم إرجاع جميع الإجازات بغض النظر عن الموظف
                       select new VacationDto
                       {
                           Id = vac.Id,
                           EmployeeId = vac.EmployeeId, // emp.Id
                           EmployeeName = emp.Name,
                           CreationDate = vac.CreationDate,
                           StartDate = vac.StartDate,
                           EndDate = vac.EndDate,
                           TypeId = vac.TypeId,
                           TypeName = lookup.Name,
                           Notes = vac.Notes
                       };

            return Ok(data);

        }

        [HttpGet("GetById")]
        public IActionResult GetById([FromQuery] long id)
        {
            var data = _dbContext.Vacations.Where(x => x.Id == id).Select(x => new VacationDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.Name,
                CreationDate = x.CreationDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TypeId = x.TypeId,
                TypeName = x.Type.Name,
                Notes = x.Notes
            }).FirstOrDefault();

            return Ok(data);
        }

        [HttpPost("Add")]
        public IActionResult Add([FromBody] SaveVacationDto saveVacationDto)
        {
            var vacation = new Vacation()
            {
                Id = 0,
                StartDate = saveVacationDto.StartDate,
                EndDate = saveVacationDto.EndDate,
                Notes = saveVacationDto.Notes,
                EmployeeId = saveVacationDto.EmployeeId,
                TypeId = saveVacationDto.TypeId,
                //CreationDate = DateTime.Now // لو ما حطيته عادي لانه معطيه default value في الموديل
            };

            _dbContext.Vacations.Add(vacation);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] SaveVacationDto saveVacationDto)
        {
            var vacation = _dbContext.Vacations.FirstOrDefault(x => x.Id == saveVacationDto.Id);
            if (vacation == null)
            {
                return BadRequest($"Vacation Not Found.");
            }
            vacation.StartDate = saveVacationDto.StartDate;
            vacation.EndDate = saveVacationDto.EndDate;
            vacation.Notes = saveVacationDto.Notes;
            vacation.EmployeeId = saveVacationDto.EmployeeId;
            vacation.TypeId = saveVacationDto.TypeId;

            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("Delete")]
        public IActionResult Delete([FromQuery] long id)
        {
            var vacation = _dbContext.Vacations.FirstOrDefault(x => x.Id == id);
            if (vacation == null)
            {
                return BadRequest($"Vacation Not Found.");
            }

            _dbContext.Vacations.Remove(vacation);
            _dbContext.SaveChanges();

            return Ok($"Vacation with Id {id} deleted successfully.");
        }

        [HttpGet("EmployeesVacationsCount")]
        public IActionResult EmployeesVacationsCount()
        {
            var data = from emp in _dbContext.Employees
                       from vac in _dbContext.Vacations.Where(x => x.EmployeeId == emp.Id).DefaultIfEmpty() // left join
                       group new { Employee = emp, Vacation = vac } by new { Id = emp.Id, Name = emp.Name } into VacationsCount // كلشي من عند from ل حد into بحطها ب VacationsCount
                       select new VacationCountDto
                       {
                           //EmployeeId = VacationsCount.Key, // معرف الموظف
                           EmployeeId = VacationsCount.Key.Id, // معرف الموظف
                           EmployeeName = VacationsCount.Key.Name, // اسم الموظف
                           //EmployeeName = VacationsCount.FirstOrDefault().Employee.Name, // اسم الموظف
                           VacationsCount = VacationsCount.ToList().Count(x => x.Vacation != null) // عدد الإجازات لكل موظف
                       };

            return Ok(data);
        }
    }
}
