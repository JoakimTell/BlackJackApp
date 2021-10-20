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
        private Deck deck;
        private List<Player> players;
        private int currentPlayer;

        public GameWindow()
        {
            InitializeComponent();
            StartNewGame();
            btnHit.IsEnabled = true;
            btnStay.IsEnabled = true;

            btnNextPlayer.IsEnabled = false;
            btnNewRound.IsEnabled = false;
        }

        private void Hit_Button_Click(object sender, RoutedEventArgs e)
        {
            PlayerNextCard();
            ScoreCheck();
        }

        private void Stay_Button_Click(object sender, RoutedEventArgs e)
        {
            currentPlayer++;
            if (currentPlayer < players.Count)
            {
                playerCard3.Source = null;
                playerCard4.Source = null;
                playerCard5.Source = null;
                playerCard6.Source = null;
                playerCard7.Source = null;
                playerCard8.Source = null;
                PlayerFirstTwoCards();
                ScoreCheck();
            }
            else
            {
                DealersSecondTurn();
                currentPlayer = 0;

                btnHit.IsEnabled = false;
                btnStay.IsEnabled = false;

                btnNextPlayer.IsEnabled = true;
                btnNewRound.IsEnabled = true;
            }
        }

        private void Shuffle_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewRound_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNextPlayer_Click(object sender, RoutedEventArgs e)
        {
            currentPlayer++;
            ScoreCheck();
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
            deck = new Deck(new List<Card>());
            players = new List<Player>();
            currentPlayer = 0;

            // Add dealer at index 0.
            players.Add(new Player("DEALER", "Dealer", new Hand(deck)));

            // Add a list of players from index 1.
            int nbrOfPlayers = 5; // TODO: Change this to value recieved from Menu!
            for (int i = 1; i <= nbrOfPlayers; i++)
            {
                players.Add(new Player(i.ToString(), $"Player {i}", new Hand(deck)));
            }

            StartNewRound();
        }

        private void StartNewRound()
        {
            // Reveal dealer first card and hide second card.
            List<Card> dealerFirstTwoCards = deck.GetTwoCards();
            players[0].Hand.AddCard(dealerFirstTwoCards[0]);
            dealerCard1.Source = RevealCard(players[0].Hand.LastCard);

            players[0].Hand.LastCard = dealerFirstTwoCards[1];
            dealerCard2.Source = HideCard();
            lblDealerScoreCalc.Content = players[0].Hand.Score;
        }

        // Set up the current player for a hand.
        private void PlayerFirstTwoCards()
        {
            List<Card> playerFirstTwoCards = deck.GetTwoCards();

            // Reveal first card.
            players[currentPlayer].Hand.AddCard(playerFirstTwoCards[0]);
            playerCard1.Source = RevealCard(players[currentPlayer].Hand.LastCard);

            // Reveal second card.
            players[currentPlayer].Hand.AddCard(playerFirstTwoCards[1]);
            playerCard2.Source = RevealCard(players[currentPlayer].Hand.LastCard);
        }

        private void PlayerNextCard()
        {
            players[currentPlayer].Hand.AddCard(deck.GetAt(0));
            deck.RemoveCard(0);
            playerCard3.Source = RevealCard(players[currentPlayer].Hand.LastCard);
        }

        // Reveal dealer second card.
        private void DealersSecondTurn()
        {
            players[0].Hand.AddCard(players[0].Hand.LastCard);
            dealerCard2.Source = RevealCard(players[0].Hand.LastCard);
            lblDealerScoreCalc.Content = players[0].Hand.Score;
        }

        private void ScoreCheck()
        {
            lblPlayerName.Content = players[currentPlayer].Name;
            lblPlayerScoreCalc.Content = players[currentPlayer].Hand.Score;

            string message = "";

            if (players[currentPlayer].Hand.Score == 21)
            {
                players[currentPlayer].Winner = true;
                message = players[currentPlayer].ToString();
            }
            else if (players[currentPlayer].Hand.Score > 21)
            {
                players[currentPlayer].Winner = false;
                message = players[currentPlayer].ToString();
            }
            else if (players[currentPlayer].Hand.Score < players[0].Hand.Score)
            {
                message = "You have LESS points than the dealer.";
            }
            else if (players[currentPlayer].Hand.Score > players[0].Hand.Score)
            {
                message = "You have More points than the dealer.";
            }
            else if (players[currentPlayer].Hand.Score == players[0].Hand.Score)
            {
                message = "You have THE SAME score as the dealer.";
            }
            lblMessage.Content = message;
        }

        private BitmapImage RevealCard(Card card)
        {
            //Value value = card.Value;
            //Suit suit = card.Suit;
            string projectPathLocal = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName; //TODO: folder depth not universal
            //string path = System.IO.Path.Combine(projectPathLocal, @"Assets\Cards\", $"{value} of {suit}.png");
            string path = System.IO.Path.Combine(projectPathLocal, @"Assets\Cards\", $"c11.png");
            return new BitmapImage(new Uri(path));
        }

        private BitmapImage HideCard()
        {
            string projectPathLocal = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName; //TODO: folder depth not universal
            string path = System.IO.Path.Combine(projectPathLocal, @"Assets\Cards\", $"cardBack_blue1.png");
            return new BitmapImage(new Uri(path));
        }
    }
}

