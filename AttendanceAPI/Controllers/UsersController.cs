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




        // Post api/user
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
        
        // GET api/users
        [HttpGet]
        public ActionResult<List<UserData>> Get()
        {
            return _context.Users.Select(u => new UserData
            {
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

        // GET api/users
        [HttpGet("department/{Department}")]
        public ActionResult<List<UserData>> GetDepartment(string Department)
        {
            return _context.Users.Where(u => u.Department == Department).Select(u => new UserData
            {
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

        // GET api/users
        [HttpGet("user/{username}")]
        public ActionResult<List<UserData>> Getid(string username)
        {
            return _context.Users.Where(u => u.Username.Equals(username)).Select(u => new UserData
            {
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

    }
}