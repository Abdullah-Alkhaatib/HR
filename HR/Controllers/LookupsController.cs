using HR.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LookupsController : ControllerBase
    {
        private readonly HrDbContext _dbContext;
        public LookupsController(HrDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpGet("GetByMajorCode")]
        public IActionResult GetByMajorCode([FromQuery] int majorCode)
        {
            try
            {
                var data = from Lookup in _dbContext.Lookups
                           where Lookup.MajorCode == majorCode && Lookup.MinorCode != 0
                           select new ListDto
                           {
                               Id = Lookup.Id,
                               Name = Lookup.Name
                           };

                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
    }
}
