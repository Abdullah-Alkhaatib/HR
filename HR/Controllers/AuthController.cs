using HR.DTOs.Auth;
using HR.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private HrDbContext _dbContext;

        public AuthController(HrDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            // ToUppter => يعني لو دخلت admin او ADMIN او AdMiN رح يرجعلي نفس ال user
            var user = _dbContext.Users.FirstOrDefault(u => u.Username.ToUpper() == loginDto.Username.ToUpper()); // يعني لو لقيت ال user اللي اسمه زي اللي دخلها
            if(user == null)
            {
                return BadRequest("Invalid username or password");
            }

            // Check Password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.HashedPassword); // يعني لو ال password اللي دخلها زي اللي مخزنه بال database
            if(!isPasswordValid)
            {
                return BadRequest("Invalid username or password");
            }

            var token = GenerateJwtToken(user);

            return Ok(token);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>(); // Claims => يعني معلومات عن ال user
            // key -> value
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())); // يعني اسم ال user
            claims.Add(new Claim(ClaimTypes.Name, user.Username)); // يعني اسم ال user

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin")); // يعني اذا ال user admin رح اعطيه صلاحيات admin
            }
            else
            {
                var employee = _dbContext.Employees.Include(x => x.Lookup).FirstOrDefault(e => e.UserId == user.Id); // يعني لو ال user مش admin رح اعطيه صلاحيات employee
                claims.Add(new Claim(ClaimTypes.Role, employee.Lookup.Name)); // يعني اذا ال user مش admin رح اعطيه صلاحيات employee
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SKDJFSJKJDSJK!!2@@@#EJSJD872DKFSKDKK"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenSettings = new JwtSecurityToken(
                claims: claims, // user info
                expires: DateTime.Now.AddDays(1), // يعني رح تنتهي صلاحية ال token بعد يوم
                signingCredentials: creds // يعني رح استخدم هاد ال key عشان اعمل sign لل token
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenSettings); // يعني رح ارجعلي ال token
            return token;
        }
    }
}