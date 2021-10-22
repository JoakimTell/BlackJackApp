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
            ButtonsIsInPlaymode(false);
        }

        private void Shuffle_Button_Click(object sender, RoutedEventArgs e)
        {
            //Shuffle...
        }

        private void btnNewRound_Click(object sender, RoutedEventArgs e)
        {
            if (players.Count > 0)
            {
                currentPlayer = 0;
                StartNewRound();
            }
            else
            {
                MessageBox.Show("Choose number of players in the menu");
            }
        }

        private void btnNextPlayer_Click(object sender, RoutedEventArgs e)
        {
            currentPlayer++;
            if (!players[0].IsFinnishied) // Before dealers second turn, compare against dealers first card.
            {
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
                    players[0].IsFinnishied = true;
                    currentPlayer = 0;
                }
            }
            else // Now compare players score to dealers final score.
            {
                if (currentPlayer < players.Count)
                {
                    ScoreCheck();
                    ButtonsIsInPlaymode(false);
                }
                else
                {
                    lblMessage.Content = "Round finnished. New game?";
                }
            }
        }

        private void New_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            deck = new Deck(new List<Card>());
            players = new List<Player>();
            Menu menu = new Menu(players, deck);
            menu.Show();
            ButtonsIsInPlaymode(false);
            btnNextPlayer.IsEnabled = false;
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

            ButtonsIsInPlaymode(false);
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
            if (!players[currentPlayer].IsFinnishied)
            {
                players[currentPlayer].Hand.AddCard(deck.GetAt(0));
                deck.RemoveCard(0);
                playerCard3.Source = RevealCard(players[currentPlayer].Hand.LastCard);
            }
        }

        // Reveal dealer second card.
        private void DealersSecondTurn()
        {
            players[0].Hand.AddCard(players[0].Hand.LastCard);
            dealerCard2.Source = RevealCard(players[0].Hand.LastCard);
            lblDealerScoreCalc.Content = players[0].Hand.Score;
        }

        // Check player score and give message.
        private void ScoreCheck()
        {
            lblPlayerName.Content = players[currentPlayer].Name;
            lblPlayerScoreCalc.Content = players[currentPlayer].Hand.Score;

            string message = "";

            if (players[currentPlayer].Hand.Score == 21)
            {
                players[currentPlayer].Winner = true;
                players[currentPlayer].IsFinnishied = true; // To not draw more cards after dealer.
                ButtonsIsInPlaymode(false);
                message = players[currentPlayer].ToString();
            }
            else if (players[currentPlayer].Hand.Score > 21)
            {
                players[currentPlayer].Winner = false;
                players[currentPlayer].IsFinnishied = true; // To not draw more cards after dealer.
                ButtonsIsInPlaymode(false);
                message = players[currentPlayer].ToString();
            }
            else
            {
                if (players[currentPlayer].Hand.Score < players[0].Hand.Score)
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
                ButtonsIsInPlaymode(true);
            }
            lblMessage.Content = message;
        }

        // Show face value of card.
        private BitmapImage RevealCard(Card card)
        {
            Value value = card.Value;
            Suit suit = card.Suit;
            string projectPathLocal = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName; //TODO: folder depth not universal
            string path = System.IO.Path.Combine(projectPathLocal, @"Assets\Cards\", $"{value}of{suit}.png");
            return new BitmapImage(new Uri(path));
        }

        // Show back of card.
        private BitmapImage HideCard()
        {
            string projectPathLocal = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName; //TODO: folder depth not universal
            string path = System.IO.Path.Combine(projectPathLocal, @"Assets\Cards\", $"cardBack_blue.png");
            return new BitmapImage(new Uri(path));
        }

        private void ButtonsIsInPlaymode(bool playing)
        {
            btnHit.IsEnabled = playing;
            btnStay.IsEnabled = playing;

            btnNextPlayer.IsEnabled = !playing;
            btnNewRound.IsEnabled = !playing;
        }
    }
}

