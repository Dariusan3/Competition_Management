using Competition_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Competition_Management.ViewModels
{
    [Keyless]
    public class TeamScore
    {
        public Team team { get; set; }

        public int Points { get; set; }

        public int Victorii { get; set; }

        public int Egaluri { get; set; }

        public int Infrangeri { get; set; }

        public int Golaveraj { get; set; }
    }
}
