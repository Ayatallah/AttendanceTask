using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceApp.Models
{
    public class UserData
    {
        public int UserId;

        [Display(Name = "Username")]
        public string Username { get; set; }
        

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Display(Name = "Email Id")]
        public string Email { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Weekly Hours")]
        public int weeklyHours { get; set; }

        public bool isAdmin { get; set; }
        public bool isDepartmentAdmin { get; set; }
    }
}
