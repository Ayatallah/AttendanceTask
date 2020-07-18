using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public string Department { get; set; }

        public bool isAdmin { get; set; }
        public bool isDepartmentAdmin { get; set; }

        public int weeklyHours { get; set; }

        public ICollection<UserLog> Logs { get; set; }
    }
}
