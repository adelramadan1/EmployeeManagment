using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
   public interface IEmployeeRepository
    {
        Employee getEmployee(int Id);
        IEnumerable<Employee> GetEmployees();
        Employee Add(Employee employee);
        Employee Delete(int Id);
        Employee Update(Employee employeeChanged);
    }
    
}
