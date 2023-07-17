using Competition_Management.Data;
using Competition_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Competition_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CompetitionManagementContext _context;

        public HomeController(ILogger<HomeController> logger, CompetitionManagementContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Competitions.Include(c => c.CompetitionTypeNavigation).ToList());
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
    }
}