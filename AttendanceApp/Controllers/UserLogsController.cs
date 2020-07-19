using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AttendanceApp.Helpers;
using AttendanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AttendanceApp.Controllers
{
    public class UserLogsController : Controller
    {
        private readonly ILogger _logger;
        AttendanceAPI _api = new AttendanceAPI();

        public UserLogsController(ILogger<UserLogsController> logger)
        {
            _logger = logger;
        }

        public ActionResult ShowCreate()
        {
            return View("Create");
        }
        public async Task<IActionResult> Create(UserLog u)
        {
            HttpClient client = _api.Initial();
            var postresult = client.PostAsJsonAsync<UserLog>("api/userlogs/open", u).Result;
            if (postresult.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", "Users", new { id = u.UserId });
            }
            else
            {
                _logger.LogInformation("Session Could not be created");
                return View();
            }
        }
        public async Task<IActionResult> ShowEdit(int id)
        {
            _logger.LogInformation("HELLOOOOOOO SHOWEDIT");
            HttpClient client = _api.Initial();
            var res = await client.GetAsync("api/userlogs/userlog/" + id.ToString());
            _logger.LogInformation("SHOWEDITTTTTTT: ", res.ToString());
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                UserLog userlog_ = JsonConvert.DeserializeObject<UserLog>(result);
               // _logger.LogInformation("SHOWEDITTTTTTT OBJJJJ: ", userlog_.UserLogId.ToString());
                return View(userlog_);
            }
            else
            {
                return RedirectToAction("Index", "Users");
            }
        }

        public async Task<IActionResult> Edit(UserLog u)
        {
            HttpClient client = _api.Initial();
            //_logger.LogInformation("EDITTTTTTTTTTT: ", u.UserLogId);
            var putresult =  client.PutAsJsonAsync<UserLog>("api/userlogs/edit", u).Result;            
            if (putresult.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", "Users", new {id= u.UserId});
            }
            else
            {
                _logger.LogInformation("Changes Could not be saved");
                return RedirectToAction("ShowEdit", "UserLogs", new { u.UserLogId});
            }
        }
    }
}