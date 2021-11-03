using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    public class PlayerContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<ChipTray> ChipTrays { get; set; }

        public static void AddNewPlayers(int nbrOfPlayers, List<Player> playerList)
        {
            using (var db = new PlayerContext())
            {
                for (int i = 1; i <= nbrOfPlayers; i++)
                {
                    db.Players.Add(playerList[i]);
                    db.ChipTrays.Add(new ChipTray());
                    db.SaveChanges();
                }
            }
        }

        public static void RemoveAllPlayers()
        {
            using (var db = new PlayerContext())
            {
                db.ChipTrays.RemoveRange(db.ChipTrays);
                db.Players.RemoveRange(db.Players);
                db.SaveChanges();
            }
        }

        public static void GetAllPlayers()
        {
            using (var db = new PlayerContext())
            {
                // Display all Players from the database
                var query = from p in db.Players
                             orderby p.Name
                             select p;

                Debug.WriteLine("All players in the database:");
                foreach (var item in query)
                {
                    Debug.WriteLine("Player name: " + item.Name);
                }
            }
        }

        public static void GetAllChipTrays()
        {
            using (var db = new PlayerContext())
            {
                // Display all Chip Trays from the database
                var query =     from c in db.ChipTrays
                                select c;

                Debug.WriteLine("All Chip Trays in the database:");
                foreach (var item in query)
                {
                    Debug.WriteLine("Chip Tray ID: " + item.ID + ". Owner: " + item.Player + ". Ones: " + item.OneDollarChips + ". Fives: " + item.FiveDollarChips + ". Twenties: " + item.TwentyDollarChips);
                }
            }
        }

        public static List<Player> FindPlayer(string searchString)
        {
            IEnumerable<Player> searchResultPlayers;

            using (var context = new PlayerContext())
            {
                // Query for all players with names containing a search string.
                searchResultPlayers =   from p in context.Players
                                        where p.Name.Contains(searchString)
                                        select p;
            }
            return searchResultPlayers.ToList();
        }
    }
}
