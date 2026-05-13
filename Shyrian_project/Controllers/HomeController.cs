using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shyrian_project.Models;
using System.Diagnostics;

namespace Shyrian_project.Controllers
{
    public class HomeController : Controller
    {

        private readonly appDbContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, appDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();

            ViewBag.FulfilledRequests = await _context.BloodRequests.CountAsync(r => r.Status == RequestStatus.Fulfilled);
            ViewBag.ActiveRequests = await _context.BloodRequests.CountAsync(r => r.Status == RequestStatus.Open);

            return View();
        }
    }
}
