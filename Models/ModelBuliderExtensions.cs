using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagment.Models
{
    public static class ModelBuliderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "Adel", Email = "Adel444@yahoo.com", Department = Department.IT,PhotoPath= "0ae81025-3460-44c8-b598-db947dcc6994_صورة٠١٠٥.jpg" },
                new Employee { Id = 2, Name = "Mohamed", Email = "Mohamed33@test.com", Department = Department.Payroll,PhotoPath= "0ae81025-3460-44c8-b598-db947dcc6994_صورة٠١٠٥.jpg" }
                );
        }
    }
}
