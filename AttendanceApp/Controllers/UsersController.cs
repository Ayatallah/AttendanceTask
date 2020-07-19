using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AttendanceApp.Helpers;
using AttendanceApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AttendanceApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger _logger;
        AttendanceAPI _api = new AttendanceAPI();

        public UsersController( ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        public ActionResult LoggedIn()
        {
            return View();
        }


        public ActionResult Logout()
        {            
            HttpClient client = _api.Initial();
            var dateTime = DateTime.Now;
            _logger.LogInformation("GETTTTT:  " + HttpContext.Session.GetString("UserId"));
            var log = new UserLog
            {
                UserId = Int32.Parse(HttpContext.Session.GetString("UserId")),
                LoginTime = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond)),
                LogoutTime = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond)),
                Status = "offline",
                LeaveDay = false,
                Holiday = false
            };
            var putresult = client.PutAsJsonAsync("api/userlogs/close", log).Result;
            if (putresult.IsSuccessStatusCode)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("LoggedIn");
            }
            else
            {
                _logger.LogInformation("Session Could not be closed");
                return View("Index");
            }
        }
        public IActionResult ViewLogin()
        {
            return View("Login");
        }
        public IActionResult ViewSearch()
        {
            return View("Search");
        }
        public IActionResult ViewRegister()
        {
            return View("Register");
        }

        public async Task<ActionResult> Search(UserSearchViewModel user)
        {
            HttpClient client = _api.Initial();
            var res = await client.GetAsync("api/users/user/" + user.Username);
            _logger.LogInformation("SEARCHHHHHH:  " + user.Username);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                UserData user_ = JsonConvert.DeserializeObject<List<UserData>>(result).FirstOrDefault();
                _logger.LogInformation("GETTTTT:  " + result.ToString());
                if(user_ != null)
                {
                    _logger.LogInformation("GETTTTT:  " + user_.ToString());

                    if (HttpContext.Session.GetString("isAdmin").Equals("False"))
                    {
                        if (!HttpContext.Session.GetString("Department").Equals(user_.Department))
                        {
                            ModelState.AddModelError("Username", "No Users Found. Please try another username search.");
                            _logger.LogInformation("User Found but in Different Department!");
                            return View();
                        }
                    }
                    return RedirectToAction("Details", new { id = user_.UserId });
                }
                else
                {
                    ModelState.AddModelError("Username", "No Users Found. Please try another username search.");
                    _logger.LogInformation("UserNOTFounnd");
                    return View();
                }
            }
            return View("Index");
        }
            public async Task<ActionResult> Login(UserLoginViewModel user)
        {
            HttpClient client = _api.Initial();
            if (user != null)
            {
                var res = await client.GetAsync("api/users/user/" + user.Username + "/" + user.Password);
                _logger.LogInformation("GETTTTT:  " + res.ToString());
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    UserData user_ = JsonConvert.DeserializeObject<UserData>(result);
                    _logger.LogInformation("GETTTTT:  " + result.ToString());
                    _logger.LogInformation("GETTTTT:  " + user_.ToString());

                    if (user_.Username != "NA")
                    {
                        HttpContext.Session.SetString("UserId", user_.UserId.ToString());
                        HttpContext.Session.SetString("Username", user_.Username.ToString());
                        HttpContext.Session.SetString("Department", user_.Department.ToString());
                        HttpContext.Session.SetString("isAdmin", user_.isAdmin.ToString());
                        HttpContext.Session.SetString("isDepartmentAdmin", user_.isDepartmentAdmin.ToString());
                        _logger.LogInformation("User Found");
                        _logger.LogInformation(user_.Username.ToString());

                        // Create Session
                        var dateTime = DateTime.Now;
                        var log = new UserLog
                        {
                            UserId = user_.UserId,
                            LoginTime = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond)),
                            LogoutTime = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond)),
                            Status = "online",
                            LeaveDay = false,
                            Holiday = false
                        };
                        var postresult = client.PostAsJsonAsync<UserLog>("api/userlogs/open", log).Result;
                        if (postresult.IsSuccessStatusCode)
                        {
                            return RedirectToAction("LoggedIn");
                        }
                        else
                        {
                            _logger.LogInformation("Session Could not be created");
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Invalid login attempt.");
                        _logger.LogInformation("UserNOTFounnd");
                        return View();
                    }
                }
                return View();
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            HttpClient client = _api.Initial();

            _logger.LogInformation("DETAILSSSSSSSS:  " + id.ToString());
            User user = new User();
            if (id == null)
            {
                return NotFound();
            }

            var res = await client.GetAsync("api/users/userdetails/" + id.ToString());
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                Tuple<User, int> u = JsonConvert.DeserializeObject<Tuple<User, int>>(result);
                user.UserId = u.Item1.UserId;
                user.Username = u.Item1.Username;
                user.FirstName = u.Item1.FirstName;
                user.LastName = u.Item1.LastName;
                user.Email = u.Item1.Email;
                user.isAdmin = u.Item1.isAdmin;
                user.isDepartmentAdmin = u.Item1.isDepartmentAdmin;
                user.Department = u.Item1.Department;
                user.weeklyHours = u.Item1.weeklyHours;
                user.Logs = u.Item1.Logs;
                user.missing = u.Item2;
                if (user == null)
                {
                    return NotFound();
                }
            }
            return View(user);
        }


        public async Task<ActionResult> Register(UserRegisterViewModel user)
        {
            HttpClient client = _api.Initial();
            var result = await client.PostAsJsonAsync<UserRegisterViewModel>("api/users/register", user);
            if(result.IsSuccessStatusCode)
            {
                return RedirectToAction("LoggedIn");
            }
            return View();
        }

        public async Task<IActionResult> Index()
        {
            List<UserData> users = new List<UserData>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res;

            // Access control to employees info based on user admin setting

            if (HttpContext.Session.GetString("isAdmin").Equals("True"))
            {
                res = await client.GetAsync("api/users");
            }
            else if (HttpContext.Session.GetString("isDepartmentAdmin").Equals("True"))
            {
                res = await client.GetAsync("api/users/department/" + HttpContext.Session.GetString("Department"));
            }
            else
            {
                res = await client.GetAsync("api/users/user/" + HttpContext.Session.GetString("Username"));
                _logger.LogInformation("GETTTTT:  " + HttpContext.Session.GetString("Username"));
            }
            _logger.LogInformation("GETTTTT:  " + res.ToString());
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserData>>(result);
                _logger.LogInformation("GETTTTT:  " + result.ToString());
                _logger.LogInformation("GETTTTT:  " + users.ToString());
            }
            return View(users);
        }
    }
}