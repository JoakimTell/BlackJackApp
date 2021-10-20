using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace BlackJackApp
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void Hit_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Stay_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Shuffle_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void New_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }

        private void StartNewGame()
        {
            //Reset the game  
            Deck deck = new Deck(new List<Card>());
            List<Player> players = new();
            
            // Add dealer at index 0.
            players.Add(new Player("DEALER", "Dealer", new Hand(deck)));
            List<Card> firstTwoCards = deck.GetTwoCards();
            players[0].Hand.AddCard(firstTwoCards[0]);
            //dealerCards.BindingGroup.Items.Add(revealCard(players[0].Hand.LastCard));
            dealerCard1.Source = revealCard(players[0].Hand.LastCard);
            players[0].Hand.AddCard(firstTwoCards[1]);
            dealerCard2.Source = revealCard(players[0].Hand.LastCard);
            lblDealerScoreCalc.Content = players[0].Hand.Score;

            // Add a list of players from index 1.
            int nbrOfPlayers = 1; // TODO: Change this to value recieved from Menu!
            for (int i = 1; i <= nbrOfPlayers; i++)
            {
                players.Add(new Player(i.ToString(), $"Player {i}", new Hand(deck)));
            }

            // Play one hand for each player, sequentially.
            for (int i = 1; i <= nbrOfPlayers; i++)
            {
                PlayOneHand(players[i], deck);
            }
        }

        private void PlayOneHand(Player player, Deck deck)
        {
            dealerCard1.Source = revealCard(player.Hand.LastCard);
            List<Card> firstTwoCards = deck.GetTwoCards();
            player.Hand.AddCard(firstTwoCards[0]);
            //dealerCards.BindingGroup.Items.Add(revealCard(players[0].Hand.LastCard));
            playerCard1.Source = revealCard(player.Hand.LastCard);
            player.Hand.AddCard(firstTwoCards[1]);
            playerCard2.Source = revealCard(player.Hand.LastCard);
            lblPlayerScoreCalc.Content = player.Hand.Score;
        }

        private BitmapImage revealCard(Card card)
        {
            //Value value = card.Value;
            //Suit suit = card.Suit;
            string projectPathLocal = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName; //TODO: folder depth not universal
            //string path = System.IO.Path.Combine(projectPathLocal, @"Assets\Cards\", $"{value} of {suit}.png");
            string path = System.IO.Path.Combine(projectPathLocal, @"Assets\Cards\", $"c11.png");
            return new BitmapImage(new Uri(path));
        }

        private void btnNewRound_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

