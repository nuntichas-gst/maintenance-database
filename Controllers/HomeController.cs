using dashboardtask.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace dashboardtask.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MyTaskIndex()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
