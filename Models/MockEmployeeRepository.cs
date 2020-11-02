using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private IList<Employee> employees;
        public MockEmployeeRepository()
        {
            employees = new List<Employee>
            {
              new Employee{Id=1,Name="Adel",Email="adel@yahoo.com",Department= Department.IT},
               new Employee{Id=2,Name="Ahmed",Email="ahmed@yahoo.com",Department= Department.IT},
                new Employee{Id=3,Name="Sara",Email="sara@yahoo.com",Department= Department.Hr},
                 new Employee{Id=4,Name="Mohamed",Email="mohamed@yahoo.com",Department= Department.Payroll}
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id = employees.Max(e => e.Id) + 1;
            employees.Add(employee);
            return employee;
        }

        public Employee Delete(int Id)
        {
            Employee employee = employees.FirstOrDefault(emp => emp.Id == Id);
            if(employee!=null)
            {
                employees.Remove(employee);
            }
           
            return employee;
        }

        public Employee getEmployee(int Id)
        {
            return employees.FirstOrDefault(emp => emp.Id == Id);
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return employees.ToList();
        }

        public Employee Update(Employee employeeChanged)
        {
            Employee employee = employees.FirstOrDefault(emp =>emp.Id==employeeChanged.Id);
            if (employee != null)
            {
                employee.Name = employeeChanged.Name;
                employee.Email = employeeChanged.Email;
                employee.Department = employeeChanged.Department;
            }
            return employee;
        }
    }
}
