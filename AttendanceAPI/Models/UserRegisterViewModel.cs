using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceAPI.Models
{
    public class UserRegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [Display(Name = "Email Id")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not Valid")]
        public string Email { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Weekly Hours")]
        public int weeklyHours { get; set; }
    }
}
