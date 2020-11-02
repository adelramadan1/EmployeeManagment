using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.ViewModels.Adminstration
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            UserClaims = new List<string>();
            UserRoles = new List<string>();
        }
        public string Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required][EmailAddress]
        public string  Email { get; set; }
        public string City { get; set; }
        public IList<string> UserClaims { get; set; }
        public List<string> UserRoles { get; set; }


    }
}
