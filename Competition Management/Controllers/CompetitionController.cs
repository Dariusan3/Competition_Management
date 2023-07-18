using Competition_Management.Data;
using Competition_Management.Models;
using Competition_Management.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using NuGet.Versioning;
using System.Collections.Generic;

namespace Competition_Management.Controllers
{
    public class CompetitionController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public CompetitionController(CompetitionManagementContext context)
        {
            _context = context;
        }

        private List<CompetitionType> GetCompetitionTypes()
        {
            // Hardcode the competition types you want to display
            List<CompetitionType> competitionTypes = new List<CompetitionType>
            {
                new CompetitionType { Id = 1, TipName = "Turneu" },
                new CompetitionType { Id = 2, TipName = "Clasament" },
            };

            return competitionTypes;
        }


        public IActionResult Index()
        {
            return View(_context.Competitions.Include(c => c.CompetitionTypeNavigation).ToList());
        }


        private List<Team> GetAvailableTeamsForCompetition(int competitionId)
        {
            Competition competition = _context.Competitions.Include(c => c.Teams).SingleOrDefault(c => c.Id == competitionId);
            List<Team> teamsAlreadyInCompetition = competition.Teams.ToList();
            List<Team> allTeams = new List<Team>();
            foreach(Competition c in _context.Competitions.Include(c => c.Teams).ToList())
            {
                foreach (Team team in c.Teams.ToList())
                {
                    allTeams = allTeams.Union(c.Teams.ToList()).ToList();
                }
            }
            List<Team> availabeTeams = new List<Team>();
            foreach (Team team in teamsAlreadyInCompetition)
            {
                allTeams.Remove(team);
            }
            availabeTeams = allTeams;
            return availabeTeams;
        }

        [HttpGet]
        public IActionResult Create()
        {

            //ViewBag.TeamDropdown.Insert(0, new SelectListItem { Text = "--Select Team--", Value = "" });
            var allTeams = _context.Teams.ToList();
            ViewData["AllTeams"] = allTeams;

            ViewBag.CompetitionTypeNavigation = new SelectList(GetCompetitionTypes(), "Id", "TipName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Competition competition, int[] teamIds, IFormFile file)
        {
            if(file != null)
            {
                byte[] byteArray;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                }
                competition.Logo = byteArray;
            }
            foreach(var teamId in teamIds)
            {
                Team team = _context.Teams.Find(teamId);
                if(team != null)
                {
                    competition.Teams.Add(team);
                }
            }

            _context.Competitions.Add(competition);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            Competition competition = _context.Competitions.Include(c => c.Teams).Include(c => c.CompetitionTypeNavigation).SingleOrDefault(c => c.Id == id);
            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Competition competition = _context.Competitions.Include(c => c.Teams).Include(c => c.Games).Include(c => c.CompetitionTypeNavigation).SingleOrDefault(c => c.Id == id);
            foreach (Team team in competition.Teams.ToList())
            {
                competition.Teams.Remove(team);
            }
            foreach(Game game in competition.Games.ToList())
            {
                competition.Games.Remove(game);
            }
            _context.Competitions.Remove(competition);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }





        [HttpGet]
        public IActionResult Edit(int? id)
        {
            Competition competition = _context.Competitions.Include(c => c.CompetitionTypeNavigation).Include(c => c.Teams).SingleOrDefault(c => c.Id == id);
            if (competition == null)
            {
                return NotFound();
            }

            //ViewBag.CompetitionTypeNavigation = new SelectList(_context.Competitions.Include(c => c.CompetitionTypeNavigation).ToList(), "Id", "CompetitionTypeNavigation.TipName");
            ViewBag.CompetitionTypeNavigation = new SelectList(GetCompetitionTypes(), "Id", "TipName");

            List<Team> availableTeams = GetAvailableTeamsForCompetition(competition.Id).ToList();

            ViewData["AllAvailableTeams"] = availableTeams;
            //ViewBag.MyLogo = Convert.ToBase64String(competition.Logo);
            return View(competition);
        }

        [HttpPost]
        public IActionResult Edit(Competition competition, int[] teamIds, IFormFile file)
        {
            if (file != null)
            {
                byte[] byteArray;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                }
                competition.Logo = byteArray;
            }

            foreach (var teamId in teamIds)
            {
                Team team = _context.Teams.Find(teamId);
                if (team != null)
                {
                    competition.Teams.Add(team);
                }
            }

            _context.Competitions.Update(competition);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int? id)
        {
            Competition competition = _context.Competitions.Include(c => c.Teams).Include(c => c.CompetitionTypeNavigation).Include(c => c.Games).ThenInclude(g => g.Team1).Include(c => c.Games).ThenInclude(g => g.Team2).SingleOrDefault(c => c.Id == id);
            if (competition == null)
            {
                return NotFound();
            }
            return View(competition);
        }


        private List<TeamScore> CalculateTeamsPoints(Competition competition)
        {
            List<TeamScore> teamScores = new List<TeamScore>();
            foreach(Team team in competition.Teams.ToList())
            {
                TeamScore teamScore = new TeamScore();
                teamScore.team = team;
                foreach (Game game in competition.Games.ToList())
                {
                    if(team.Id == game.Team1Id)
                    {
                        teamScore.Golaveraj += (int)game.Team1Goals;
                        teamScore.Golaveraj -= (int)game.Team2Goals;
                        if (game.Team1Goals >= game.Team2Goals)
                        {
                            if(game.Team1Goals == game.Team2Goals)
                            {
                                teamScore.Egaluri++;
                                teamScore.Points += 1;
                            }
                            else
                            {
                                teamScore.Victorii++;
                                teamScore.Points += 3;
                            }
                        }
                        else
                        {
                            teamScore.Infrangeri++;
                            continue;
                        }
                    }
                    else if(team.Id == game.Team2Id)
                    {
                        teamScore.Golaveraj += (int)game.Team2Goals;
                        teamScore.Golaveraj -= (int)game.Team1Goals;
                        if (game.Team1Goals <= game.Team2Goals)
                        {
                            if (game.Team1Goals == game.Team2Goals)
                            {
                                teamScore.Egaluri++;
                                teamScore.Points += 1;
                            }
                            else
                            {
                                teamScore.Victorii++;
                                teamScore.Points += 3;
                            }
                        }
                        else
                        {
                            teamScore.Infrangeri++;
                            continue;
                        }
                    }
                }
                teamScores.Add(teamScore);
            }
            return teamScores;
        }



        [HttpGet]
        public IActionResult Clasament(int? id)
        {
            Competition competition = _context.Competitions.Include(c => c.Teams).Include(c => c.CompetitionTypeNavigation).Include(c => c.Games).ThenInclude(c => c.Team1).Include(c => c.Games).ThenInclude(c => c.Team2).SingleOrDefault(c => c.Id == id);
            if (competition == null)
            {
                return NotFound();
            }
            ViewData["CompetitionName"] = competition.CompetitionName;
            List<TeamScore> teamScores = CalculateTeamsPoints(competition);
            teamScores = teamScores.OrderByDescending(t => t.Points).ToList();

            return View(teamScores);
        }

        [HttpPost]
        public IActionResult GenerateGames(int? id)
        {
            Competition competition = _context.Competitions.Include(c => c.Teams).Include(c => c.Games).SingleOrDefault(c => c.Id == id);
            if (competition == null)
            {
                return NotFound();
            }
            competition.Games.Clear();

            List<Team> teams = competition.Teams.ToList();
            List<Game> games = new List<Game>();

            // Generate games for each team against all other teams
            for (int i = 0; i < teams.Count - 1; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    Random random1 = new Random();
                    int goalsTeam1 = random1.Next(0, 4);
                    int goalsTeam2 = random1.Next(0, 2);
                    // Create a game where Team1 plays against Team2
                    Game game1 = new Game
                    {
                        CompetitionId = competition.Id,
                        Team1Id = teams[i].Id,
                        Team2Id = teams[j].Id,
                        Team1Goals = goalsTeam1,
                        Team2Goals = goalsTeam2,
                    };
                    goalsTeam1 = random1.Next(0, 4);
                    goalsTeam2 = random1.Next(0, 2);
                    // Create a game where Team2 plays against Team1 (away game)
                    Game game2 = new Game
                    {
                        CompetitionId = competition.Id,
                        Team1Id = teams[j].Id,
                        Team2Id = teams[i].Id,
                        Team1Goals = goalsTeam1,
                        Team2Goals = goalsTeam2,
                    };

                    games.Add(game1);
                    games.Add(game2);
                }
            }

            // Shuffle the games randomly
            Random random = new Random();
            games = games.OrderBy(g => random.Next()).ToList();

            // Assign start and end dates to the games
            DateTime startDate = (DateTime)competition.StartDate;
            foreach (Game game in games)
            {
                game.Date = startDate;
                game.Date = startDate.AddDays(7); // Assuming each game lasts for 7 days
                startDate = startDate.AddDays(7);
            }

            // Add the generated games to the competition
            competition.Games.AddRange(games);

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = competition.Id });
        }

    }
}
