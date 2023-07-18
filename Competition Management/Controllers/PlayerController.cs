using Competition_Management.Data;
using Competition_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;


namespace Competition_Management.Controllers
{
    public class PlayerController : Controller
    {

        private readonly CompetitionManagementContext _context;

        public PlayerController(CompetitionManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index(string sortOrder, string searchString, int searchTeam)
        {
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.AgeSortParam = sortOrder == "Age" ? "age_desc" : "Age";
            var teams = _context.Teams.ToList();
            Team team_free_contract = new Team
            {
                Id = -1,
                TeamName = "Free Contract",
            };
            teams.Add(team_free_contract);
            ViewBag.Teams = new SelectList(teams, "Id", "TeamName");
            var players = from p in _context.Players.Include(p => p.Team) select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                players = players.Where(p => p.LastName.Contains(searchString)
                || p.FirstName.Contains(searchString));
            }
            if (searchTeam != null && searchTeam != 0 && searchTeam != -1)
            {
                players = players.Where(p => p.TeamId == searchTeam);
            }
            if(searchTeam == -1)
            {
                players = players.Where(p => p.TeamId == null);
            }

            //players = players.Where(p => p.Team.TeamName.Contains(searchTeam));
            switch (sortOrder)
            {
                case "name_desc":
                    players = players.OrderByDescending(p => p.LastName); 
                    break;
                case "Age":
                    players = players.OrderBy(p => p.Age);
                    break;
                case "age_desc":
                    players = players.OrderByDescending(p => p.Age);
                    break;
                default:
                    players = players.OrderBy(p => p.LastName);
                    break;
    
            } 

            return View(players.ToList());
        }

        public IActionResult Create()
        {
            var teams = _context.Teams.ToList();
            Team team_free_contract = new Team
            {
                Id = -1,
                TeamName = "Free Contract",
            };
            teams.Add(team_free_contract);
            ViewBag.TeamDropdown = new SelectList(teams, "Id", "TeamName");
            //ViewBag.TeamDropdown.Insert(0, new SelectListItem { Text = "--Select Team--", Value = "" });
               
            return View();
        }

        [HttpPost]
        public IActionResult Create(Player player, IFormFile file)
        {

            if (file != null)
            {
                byte[] byteArray;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                }
                player.Photo = byteArray;
            }
            if(player.TeamId == -1)
            {
                player.TeamId = null;
            }
            _context.Players.Add(player);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            Player player = _context.Players.Include(p => p.Team).SingleOrDefault(p => p.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        [HttpPost]
        public IActionResult Delete(Player player)
        {
            _context.Players.Remove(player);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ViewBag.TeamDropdown = new SelectList(_context.Teams.ToList(), "Id", "TeamName");
            Player player = _context.Players.Find(id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }

        [HttpPost]
        public IActionResult Edit(Player player, int TeamId, IFormFile file)
        {
            if (file != null)
            {
                byte[] byteArray;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                }
                player.Photo = byteArray;
            }

            Team team = _context.Teams.SingleOrDefault(t => t.Id == TeamId);
            player.Team = team;
            player.TeamId = TeamId;
            
            _context.Players.Update(player);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Details(int? id)
        {
            Player player = _context.Players.Include(p => p.Team).SingleOrDefault(p=>p.Id == id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }
    }
}
