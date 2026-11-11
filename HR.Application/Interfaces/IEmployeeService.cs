using HR.DTOs.Employees;
using System;
using System.Collections.Generic;
using System.Text;

namespace HR.Application.Interfaces
{
    public interface IEmployeeService
    {
        public IEnumerable<EmployeeDto> GetAll(FilterEmployeeDto filterDto);
    }
}
