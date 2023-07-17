using Competition_Management.Data;
using Competition_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Competition_Management.Controllers
{
    public class GameController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public GameController(CompetitionManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Game> g1 = _context.Games.Include(j => j.Competition).ToList();
            List<Game> g2 = _context.Games.Include(j => j.Team1).ToList();
            List<Game> g3 = _context.Games.Include(j => j.Team2).ToList();
            List<Game> game = Enumerable.Union(g1, g2).ToList();
            game = game.Union(g3).ToList();
            return View(game);
        }

        public IActionResult Create(int? CompId)
        {
            Competition competition = _context.Competitions.Include(c => c.Teams).SingleOrDefault(c => c.Id == CompId);
            ViewBag.Team1Dropdown = new SelectList(competition.Teams.ToList(), "Id", "TeamName");
            ViewBag.Team2Dropdown = new SelectList(competition.Teams.ToList(), "Id", "TeamName");
           
            if (competition == null)
            {
                return NotFound();
            }

            ///ViewData["CompetitionName"] = competition.CompetitionName as string;
            ViewBag.CompetitionName = competition.CompetitionName as string;
            //ViewBag.TeamDropdown.Insert(0, new SelectListItem { Text = "--Select Team--", Value = "" });
            ViewBag.CompetitionId = competition.Id;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Game game, int? CompId)
        {
            Competition competition = _context.Competitions.Include(c => c.Teams).SingleOrDefault(c => c.Id == CompId);
            game.CompetitionId = CompId;
            game.CompetetitionName = competition.CompetitionName;
            Team team1 = _context.Teams.Find(game.Team1Id);
            Team team2 = _context.Teams.Find(game.Team2Id);
            game.Team1Name = team1.TeamName;
            game.Team2Name = team2.TeamName;
            _context.Games.Add(game);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            Game game = _context.Games.Include(j => j.Team1).Include(j => j.Team2).Include(j => j.Competition).SingleOrDefault(j => j.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        [HttpPost]
        public IActionResult Delete(Game game)
        {
            _context.Games.Remove(game);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Details(int? id)
        {
            Game game = _context.Games.Include(j => j.Team1).Include(j => j.Team2).Include(j => j.Competition).SingleOrDefault(j => j.Id == id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }
    }
}
