using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.ViewModels.Adminstration
{
    public class RoleCreateViewModel
    {
        [Required(ErrorMessage ="Role Field Is Required")]
        public string RoleName { get; set; }
    }
}
