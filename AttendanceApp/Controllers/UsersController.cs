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

        public IActionResult ViewRegister()
        {
            return View("Register");
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

        public async Task<ActionResult> Register(UserRegisterViewModel user)
        {
            HttpClient client = _api.Initial();
            //var postTask = await client.PostAsJsonAsync<UserRegisterViewModel>("api/users/register", user);
            //postTask.Wait();
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
            //res = await client.GetAsync("api/users");
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