using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    public class PlayerContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<ChipTray> ChipTrays { get; set; }
    }
}
