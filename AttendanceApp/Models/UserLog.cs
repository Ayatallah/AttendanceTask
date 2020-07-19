using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceApp.Models
{
    public class UserLog
    {
        public int UserLogId { get; set; }

        [Display(Name = "Login Time")]
        public DateTime LoginTime { get; set; }
        [Display(Name = "Logout Time")]
        public DateTime LogoutTime { get; set; }

        public string Status { get; set; }
        [Display(Name = "Leave Day")]
        public bool LeaveDay { get; set; }
        [Display(Name = "Holiday")]
        public bool Holiday { get; set; }

        public int UserId { get; set; }

        //public User user { get; set; }
    }
}
