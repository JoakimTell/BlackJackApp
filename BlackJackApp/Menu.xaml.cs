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

namespace BlackJackApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Menu : Window
    {
        List<Player> players;
        Deck deck;
        public Menu(List<Player> players, Deck deck)
        {
            InitializeComponent();
            this.players = players;
            this.deck = deck;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int nmbrOfPlayers = Int32.Parse(txtNmbrOfPlayers.Text);

            // Add dealer at index 0.
            players.Add(new Player("DEALER", "Dealer", new Hand(deck)));
           
            // Add a list of players from index 1.
            for (int i = 1; i <= nmbrOfPlayers; i++)
            {
                players.Add(new Player(i.ToString(), $"Player {i}", new Hand(deck)));
                Debug.WriteLine("Player number " + i + " added");
            }
            this.Close();
        }
    }
}
