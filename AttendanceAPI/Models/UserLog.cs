using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceAPI.Models
{
    public class UserLog
    {
        [Key]
        public int UserLogId { get; set; }

        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }

        public string Status { get; set; }
        public bool LeaveDay { get; set; }
        public bool Holiday { get; set; }

        public int UserId { get; set; }

        public User user { get; set; }
    }
}
