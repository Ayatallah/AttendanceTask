using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLogsController : ControllerBase
    {
        private readonly DataContext _context;

        public UserLogsController(DataContext context)
        {
            _context = context;
        }
        
        [HttpPost("open")]
        public async Task<ActionResult> OpenSession(UserLog userlog)
        {
            if (ModelState.IsValid)
            {
                AttendanceAPI.Models.UserLog u = new AttendanceAPI.Models.UserLog
                {
                    LoginTime = userlog.LoginTime,
                    LogoutTime = userlog.LogoutTime,
                    Status = userlog.Status,
                    LeaveDay = userlog.LeaveDay,
                    Holiday = userlog.Holiday,
                    UserId = userlog.UserId
                };
                _context.UserLogs.Add(u);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                ModelState.AddModelError("", "Some Error Occured!");
                return BadRequest("Not a Valid Model");
            }
        }
        
        [HttpPut("close")]
        public async Task<ActionResult> CloseSession(UserLog userlog)
        {
            var logs_ = await _context.UserLogs.SingleOrDefaultAsync(u => u.UserId.Equals(userlog.UserId) && u.Status.Equals("online"));
            if(logs_ != null)
            {
                logs_.LogoutTime = userlog.LogoutTime;
                logs_.Status = userlog.Status;
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                ModelState.AddModelError("", "Some Error Occured!");
                return BadRequest("Session Not Found");
            }
        }
        
        [HttpPut("edit")]
        public async Task<ActionResult> EditSession(UserLog userlog)
        {
            var logs_ = await _context.UserLogs.SingleOrDefaultAsync(u => u.UserLogId == userlog.UserLogId);
            if (logs_ != null)
            {
                logs_.LoginTime = userlog.LoginTime;
                logs_.LogoutTime = userlog.LogoutTime;
                logs_.LeaveDay = userlog.LeaveDay;
                logs_.Holiday = userlog.Holiday;
                logs_.Status = userlog.Status;
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                ModelState.AddModelError("", "Some Error Occured!");
                return BadRequest("Session Not Found");
            }
        }

        [HttpGet("userlog/{userlogid}")]
        public ActionResult<UserLog> GetSession(string userlogid)
        {
            return _context.UserLogs.Where(u => u.UserLogId == Int32.Parse(userlogid)).Select(u => new UserLog
            {
                UserLogId = u.UserLogId,
                UserId = u.UserId,
                LoginTime = u.LoginTime,
                LogoutTime = u.LogoutTime,
                LeaveDay = u.LeaveDay,
                Holiday = u.Holiday,
                Status = u.Status
        }).FirstOrDefault<UserLog>();
        }
    }
}