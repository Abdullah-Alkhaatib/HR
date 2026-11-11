using HR.Application.Interfaces;
using HR.DTOs.Employees;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace HR.Infra.Services
{
    public class EmployeeService : IEmployeeService
    {
        private HrDbContext _dbContext;
        private IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public EmployeeService(HrDbContext dbContext, IWebHostEnvironment env, IConfiguration config)
        {
            _dbContext = dbContext;
            _env = env;
            _config = config;
        }
        public IEnumerable<EmployeeDto> GetAll(FilterEmployeeDto filterDto) // Query parameter, Position is optional parameter
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
                           ImagePath = employee.ImagePath != null ? Path.Combine(_config["BaseUrl"], employee.ImagePath) : "",
                           UserId = employee.UserId
                       };

            return data;
        }
    }
}
