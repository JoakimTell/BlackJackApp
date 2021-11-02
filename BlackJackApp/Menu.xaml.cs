using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using UtilitiesLib;

namespace BlackJackApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Menu : Window
    {
        ListManager<Player> players;
        Deck deck;

        public Menu(ListManager<Player> players, Deck deck)
        {
            InitializeComponent();
            this.players = players;
            this.deck = deck;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool okPlayers = int.TryParse(txtNmbrOfPlayers.Text.Trim(), out int nbrOfPlayers);
            bool okDecks = int.TryParse(txtNmbrOfDecks.Text.Trim(), out int nbrOfDecks);

            if (okPlayers && okDecks)
            {
                // Add dealer at index 0.
                Player dealer = new Player("Dealer", new Hand(deck));
                players.Add(dealer);
                
                deck.InitializeNewDeck(nbrOfDecks);
                deck.Shuffle();

                Debug.WriteLine("");
                Debug.WriteLine("");
                // Add a list of players from index 1.
                for (int i = 1; i <= nbrOfPlayers; i++)
                {
                    string name = $"Player {i}";
                    Player player = new Player(name, new Hand(deck));
                    players.Add(player);

                    Debug.WriteLine("Player '" + name + "' added.");
                    
                }

                using (var db = new PlayerContext())
                {
                    db.ChipTrays.RemoveRange(db.ChipTrays);
                    db.Players.RemoveRange(db.Players);
                    db.SaveChanges();

                    for (int i = 1; i <= nbrOfPlayers; i++)
                    {
                        db.Players.Add(players.List[i]);
                        db.ChipTrays.Add(new ChipTray());
                        db.SaveChanges();
                    }

                    // Display all Players from the database
                    var query1 = from p in db.Players
                                 orderby p.Name
                                 select p;

                    Debug.WriteLine("All players in the database:");

                    foreach (var item in query1)
                    {
                        Debug.WriteLine("Player name: " + item.Name);
                    }

                    // Display all Chip Trays from the database
                    var query2 = from c in db.ChipTrays
                                 select c;

                    Debug.WriteLine("All Chip Trays in the database:");
                    foreach (var item in query2)
                    {
                        Debug.WriteLine("Chip Tray ID: " + item.ID);
                    }
                }

                Debug.WriteLine("");
                foreach (Player pl in players.List)
                {
                    deck.DeckIsRunningOut += pl.On_Deck_LowOnCards_Player;
                }
                Close();
            }
        }
    }
}
