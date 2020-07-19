using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceApp.Models
{
    public class UserSearchViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
    }
}
