using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagment.Models;

namespace EmployeeManagment.ViewModels.Adminstration
{
    public class RoleEditViewModel
    {
        public RoleEditViewModel()
        {
            Users = new List<string>();
        }
        public string Id { get; set; }
        [Required(ErrorMessage ="Role Name is rquired")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}
