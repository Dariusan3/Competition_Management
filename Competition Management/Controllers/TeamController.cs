using Competition_Management.Data;
using Competition_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Numerics;

namespace Competition_Management.Controllers
{
    public class TeamController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public TeamController(CompetitionManagementContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View(_context.Teams.ToList());
        }


        private List<Player> GetAvailablePlayers(List<Player> allPlayers)
        {
            List<Player> availablePLayers = new List<Player>();
            foreach (Player player in allPlayers)
            {
                if (player.TeamId == null)
                {
                    availablePLayers.Add(player);
                }
            }
            return availablePLayers;
        }

        [HttpGet]
        public IActionResult Create()
        {
            List<Player> allPlayers = _context.Players.ToList();
            List<Player> availablePLayers = GetAvailablePlayers(allPlayers);
            availablePLayers = availablePLayers.OrderBy(x => x.LastName).ToList();
            ViewData["AllAvailablePlayers"] = availablePLayers;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Team team, int[] playerIds, IFormFile file)
        {
            foreach(Team t in _context.Teams.ToList())
            {
                if (t.TeamName.ToLower().CompareTo(team.TeamName.ToLower()) == 0)
                {
                    ModelState.AddModelError("TeamName", "Ai introdus un nume pentru echipa deja existent");
                    List<Player> allPlayers = _context.Players.ToList();
                    List<Player> availablePLayers = GetAvailablePlayers(allPlayers);
                    availablePLayers = availablePLayers.OrderBy(x => x.LastName).ToList();
                    ViewData["AllAvailablePlayers"] = availablePLayers;
                    return View();
                }
            }
            if (file != null)
            {
                byte[] byteArray;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                }
                team.Sigla = byteArray;
            }

            foreach (var playerId in playerIds)
            {
                Player player = _context.Players.Find(playerId);
                if (player != null)
                {
                    team.Players.Add(player);
                }
            }
            _context.Add(team);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            Team team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Team team = _context.Teams.Include(c => c.Players).Include(t => t.GameTeam1s).Include(t => t.GameTeam2s).SingleOrDefault(t => t.Id == id);
            foreach (Player player in team.Players.ToList())
            {
                team.Players.Remove(player);
            }
            foreach(Competition competition in _context.Competitions.Include(c => c.Teams))
            {
                Team teamToDelete = competition.Teams.SingleOrDefault(t => t.Id == id);
                competition.Teams.Remove(teamToDelete);
            }
            _context.Teams.Remove(team);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            Team team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        [HttpPost]
        public IActionResult Edit(Team team)
        {
            _context.Teams.Update(team);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Details(int? id)
        {
            Team team = _context.Teams.Include(t => t.Players).SingleOrDefault(t => t.Id == id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

    }
}
