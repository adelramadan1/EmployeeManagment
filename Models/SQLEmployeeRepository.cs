using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagment.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext context;
        public SQLEmployeeRepository(AppDbContext context)
        {
           this.context = context;
        }
        public Employee Add(Employee employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public Employee Delete(int Id)
        {
            Employee employee = context.Employees.Find(Id);
            if(employee!=null)
            {
                context.Employees.Remove(employee);
                context.SaveChanges();
            }
            return employee;
        }

        public Employee getEmployee(int Id)
        {
            return context.Employees.Find(Id);
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return context.Employees;
        }

        public Employee Update(Employee employeeChanged)
        {
            var employee = context.Employees.Attach(employeeChanged);
            employee.State = EntityState.Modified;
            context.SaveChanges();
            return employeeChanged;

        }
    }
}
