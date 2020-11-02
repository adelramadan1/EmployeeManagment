using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagment.Models;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagment.ViewModels
{
    public class EmployeeCreateViewModel
    {
        [Required(ErrorMessage = "the Name field is rquired")]
        [MaxLength(10, ErrorMessage = "the Name Must not More 10 Characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Choose Your Department")]

        public Department? Department { get; set; }
        [Required(ErrorMessage = "th email field is required")]
        [Display(Name = "Office Email")]
        public string Email { get; set; }
        public IFormFile Photo { get; set; }
    }
}
