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
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("user/{Username}/{Password}")]
        public ActionResult<UserData> GetUser(string Username, string Password)
        {
            var users_ =  _context.Users.Where(u => u.Username.Equals(Username) && u.Password.Equals(Password)).FirstOrDefault<User>();
            if(users_ != null)
            {
                UserData u = new UserData
                {
                    UserId = users_.UserId,
                    Username = users_.Username,
                    FirstName = users_.FirstName,
                    LastName = users_.LastName,
                    Email = users_.Email,
                    Department = users_.Department,
                    weeklyHours = users_.weeklyHours,
                    isAdmin = users_.isAdmin,
                    isDepartmentAdmin = users_.isDepartmentAdmin
                };
                return u;
            }
            else
            {
                UserData u = new UserData
                {
                    UserId = 0,
                    Username = "NA",
                    FirstName = "NA",
                    LastName = "NA",
                    Email = "NA",
                    Department = "NA",
                    weeklyHours = 0,
                    isAdmin = false,
                    isDepartmentAdmin = false
                };
                return u;
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterViewModel user)
        {
            if (ModelState.IsValid)
            {
                AttendanceAPI.Models.User u = new AttendanceAPI.Models.User
                {
                    Username = user.Username,
                    Password = user.Password,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Department = user.Department,
                    weeklyHours = user.weeklyHours
                };
                _context.Users.Add(u);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                ModelState.AddModelError("", "Some Error Occured!");
                return BadRequest("Not a Valid Model");
            }
        }

        [HttpGet]
        public ActionResult<List<UserData>> Get()
        {
            return _context.Users.Select(u => new UserData
            {
                UserId = u.UserId,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Department = u.Department,
                weeklyHours = u.weeklyHours,
                isAdmin = u.isAdmin,
                isDepartmentAdmin = u.isDepartmentAdmin
            }).ToList() ;
        }

        [HttpGet("department/{Department}")]
        public ActionResult<List<UserData>> GetDepartment(string Department)
        {
            return _context.Users.Where(u => u.Department == Department).Select(u => new UserData
            {
                UserId = u.UserId,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Department = u.Department,
                weeklyHours = u.weeklyHours,
                isAdmin = u.isAdmin,
                isDepartmentAdmin = u.isDepartmentAdmin
            }).ToList();
        }
        
        [HttpGet("user/{username}")]
        public ActionResult<List<UserData>> Getid(string username)
        {
            return _context.Users.Where(u => u.Username.Equals(username)).Select(u => new UserData
            {
                UserId = u.UserId,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Department = u.Department,
                weeklyHours = u.weeklyHours,
                isAdmin = u.isAdmin,
                isDepartmentAdmin = u.isDepartmentAdmin
            }).ToList();
        }
        
        [HttpGet("userdetails/{userid}")]
        public ActionResult<Tuple<User, int>> Getdetails(string userid)
        {
            User user_ =  _context.Users.Where(u => u.UserId == Int32.Parse(userid)).Select(u => new User
            {
                UserId = u.UserId,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Department = u.Department,
                weeklyHours = u.weeklyHours,
                isAdmin = u.isAdmin,
                isDepartmentAdmin = u.isDepartmentAdmin,
                Logs = u.Logs
            }).FirstOrDefault<User>();
            var missing = 0;
            foreach(var log in user_.Logs)
            {
                if(log.LoginTime.Month == DateTime.Now.Month)
                {
                    if(log.Holiday==true || log.LeaveDay == true)
                    {
                        missing += 480;
                    }
                    else
                    {
                        TimeSpan span = log.LogoutTime.Subtract(log.LoginTime);
                        missing += Convert.ToInt32(span.TotalMinutes);
                    }
                }
            }
            missing = missing / 60;
            missing = (user_.weeklyHours*4) - missing;
            return Tuple.Create(user_, missing);
        }

    }
}