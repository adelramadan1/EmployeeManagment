using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagment.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagment.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [ValidEmailDomain(allowedDomain:"test.com",ErrorMessage ="Email Domain mest be test.com")]
        [Remote(action: "IsEmailInUse",controller:"Account")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password",ErrorMessage ="Password and  Confirm  Passwordis not match")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage ="Enter Your City please.")]
        public string City { get; set; }
    }
}
