using Microsoft.Win32;
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
using UtilitiesLib;

namespace BlackJackApp
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private Deck deck;
        private ListManager<Player> players;
        private int currentPlayer;
        private static int dealer;

        public GameWindow()
        {
            InitializeComponent();
            btnNextPlayer.IsEnabled = false;
            btnNewRound.IsEnabled = false;
        }

        private void Hit_Button_Click(object sender, RoutedEventArgs e)
        {
            Hit();
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
            btnNextPlayer.IsEnabled = true;
            if (players.Count > 0)
            {
                if (deck.GameIsDone)
                {
                    currentPlayer = 0;
                    StartNewRound();
                }
                else
                {
                    MessageBox.Show("Finnish the started round first.");
                }
            }
            else
            {
                MessageBox.Show("Choose number of players in the menu.");
            }
        }

        private void btnNextPlayer_Click(object sender, RoutedEventArgs e)
        {
            currentPlayer++;
            btnNextPlayer.Content = "Next Player";

            if (!players.GetAt(dealer).IsFinnishied) // Before dealers second turn, compare against dealers first card.
            {
                if (currentPlayer < players.Count)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
                        nextImage.Source = null;
                    }
                    PlayerFirstTwoCards();
                    ScoreCheck();
                    if (currentPlayer == players.Count - 1)
                    {
                        btnNextPlayer.Content = "Dealers round";
                    }
                }
                else
                {
                    DealersSecondRound();
                    players.GetAt(dealer).IsFinnishied = true;
                    currentPlayer = 0;
                    lblMessage.Content = players.GetAt(dealer).Hand.Score > 21
                        ? "Dealer busts. Remaining players win. Compare scores."
                        : "Dealer stays. Compare scores.";
                    lblPlayerName.Content = "";
                    lblPlayerScoreCalc.Content = "";
                    for (int i = 0; i < 8; i++)
                    {
                        Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
                        nextImage.Source = null;
                    }
                    btnNextPlayer.Content = "Compare score";
                }
            }
            else // Now compare players score to dealers final score.
            {
                if (currentPlayer < players.Count)
                {
                    for (int i = 0; i < players.GetAt(currentPlayer).Hand.Cards.Count; i++)
                    {
                        Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
                        nextImage.Source = RevealCard(players.GetAt(currentPlayer).Hand.Cards[i]);
                    }
                    ScoreCheck();
                    ButtonsIsInPlaymode(false);
                    if (currentPlayer == players.Count - 1)
                    {
                        btnNextPlayer.Content = "End Round";
                    }
                }
                else
                {
                    deck.GameIsDone = true;
                    lblMessage.Content = "Round finnished. New game?";
                    btnNextPlayer.Content = "Next Player";
                    btnNextPlayer.IsEnabled = false;
                    foreach (Player player in players.List)
                    {
                        player.Hand.Clear();
                        player.IsFinnishied = true;
                    }
                }
            }
        }



        private void New_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            deck = new Deck(new List<Card>());
            deck.GameIsDone = true;
            players = new ListManager<Player>();
            Menu menu = new Menu(players, deck);
            menu.Show();
            ButtonsIsInPlaymode(false);
            btnNextPlayer.IsEnabled = false;
        }

        public void AddPlayersToListView()
        {
            if (lstViewPlayerProgress.Items.Count == 0)
            {
                foreach (Player player in players.List)
                {
                    var row = new { PlayerID = player.PlayerID, Wins = player.Wins, Losses = player.Losses };
                    lstViewPlayerProgress.Items.Add(player);
                }
            }
            else
            {
                lstViewPlayerProgress.Items.Refresh();
            
                
            }

        }

        private void StartNewRound()
        {
            AddPlayersToListView();
            lblPlayerName.Content = "";
            lblPlayerScoreCalc.Content = "";
            lblMessage.Content = "Dealer is dealt first cards. Next player may start to play.";
            for (int i = 0; i < 8; i++)
            {
                Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
                nextImage.Source = null;
                nextImage = VisualTreeHelper.GetChild(canvasDealerCards, i) as Image;
                nextImage.Source = null;
            }

            deck.GameIsDone = false;
            foreach (Player player in players.List)
            {
                player.IsFinnishied = false;
            }
            DealersFirstTwoCards();

            ButtonsIsInPlaymode(false);
        }

        // Set up the current player for a hand.
        private void PlayerFirstTwoCards()
        {
            Hand hand = players.GetAt(currentPlayer).Hand;
            List<Card> playerFirstTwoCards = deck.GetTwoCards();
            Image nextImage;

            // Reveal the players first two cards.
            for (int i = 0; i < 2; i++)
            {
                hand.AddCard(playerFirstTwoCards[i]);
                nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
                nextImage.Source = RevealCard(hand.LastCard);
            }
        }

        private void Hit()
        {
            if (!players.GetAt(currentPlayer).IsFinnishied)
            {
                Hand hand = players.GetAt(currentPlayer).Hand;
                Card card = deck.GetAt(0);

                hand.AddCard(card);
                deck.RemoveCard(0);

                Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, hand.NumberOfCards) as Image;
                nextImage.Source = RevealCard(hand.LastCard);
            }
        }

        // Dealer is dealt its first revealed card, and its hidden second card.
        private void DealersFirstTwoCards()
        {
            Hand hand = players.GetAt(dealer).Hand;
            List<Card> dealerFirstTwoCards = deck.GetTwoCards();

            // Reaveal first added card.
            hand.AddCard(dealerFirstTwoCards[0]);
            dealerCard1.Source = RevealCard(hand.LastCard);

            // Hide second added card.
            hand.LastCard = dealerFirstTwoCards[1];
            dealerCard2.Source = HideCard();

            lblDealerScoreCalc.Content = hand.Score;
        }

        // Dealer plays until score is between 15 to 21, or gets busted.
        private void DealersSecondRound()
        {
            Hand hand = players.GetAt(dealer).Hand;

            // Reaveal second added card.
            hand.AddCard(hand.LastCard);
            dealerCard2.Source = RevealCard(hand.LastCard);

            // Add more cards.
            while (hand.Score < 15)
            {
                Card card = deck.GetAt(0);
                hand.AddCard(card);
                deck.RemoveCard(0);

                // Show a new card for the dealer.
                Image nextImage = VisualTreeHelper.GetChild(canvasDealerCards, hand.NumberOfCards) as Image;
                nextImage.Source = RevealCard(hand.LastCard);
            }

            lblDealerScoreCalc.Content = hand.Score;
        }

        // Check player score and give message.
        private void ScoreCheck()
        {
            lblPlayerName.Content = players.GetAt(currentPlayer).Name;
            lblPlayerScoreCalc.Content = players.GetAt(currentPlayer).Hand.Score;

            string message = "";

            if (!players.GetAt(dealer).IsFinnishied)
            {
                if (players.GetAt(currentPlayer).Hand.Score == 21)
                {
                    players.GetAt(currentPlayer).Winner = true;
                    players.GetAt(currentPlayer).Wins++;
                    players.GetAt(currentPlayer).IsFinnishied = true; // To not draw more cards after dealer.
                    ButtonsIsInPlaymode(false);
                    message = players.GetAt(currentPlayer).ToString();
                }
                else if (players.GetAt(currentPlayer).Hand.Score > 21)
                {
                    players.GetAt(currentPlayer).Winner = false;
                    players.GetAt(currentPlayer).Losses++;
                    players.GetAt(currentPlayer).IsFinnishied = true; // To not draw more cards after dealer.
                    ButtonsIsInPlaymode(false);
                    message = players.GetAt(currentPlayer).ToString();
                }
                else
                {
                    if (players.GetAt(currentPlayer).Hand.Score < players.GetAt(dealer).Hand.Score)
                    {
                        message = "You have LESS points than the dealer.";
                    }
                    else if (players.GetAt(currentPlayer).Hand.Score > players.GetAt(dealer).Hand.Score)
                    {
                        message = "You have More points than the dealer.";
                    }
                    else if (players.GetAt(currentPlayer).Hand.Score == players.GetAt(dealer).Hand.Score)
                    {
                        message = "You have THE SAME score as the dealer.";
                    }
                    ButtonsIsInPlaymode(true);
                }
            }
            else
            {
                if (players.GetAt(currentPlayer).IsFinnishied)
                {
                    message = players.GetAt(currentPlayer).ToString();
                }
                else if (players.GetAt(dealer).Hand.Score > 21)
                {
                    players.GetAt(currentPlayer).IsFinnishied = true;
                    players.GetAt(currentPlayer).Winner = true;
                    players.GetAt(dealer).Losses++;
                    players.GetAt(currentPlayer).Wins++;
                    message = players.GetAt(currentPlayer).ToString();
                }
                else if (players.GetAt(currentPlayer).Hand.Score < players.GetAt(dealer).Hand.Score)
                {
                    players.GetAt(currentPlayer).IsFinnishied = true;
                    players.GetAt(currentPlayer).Winner = false;
                    players.GetAt(currentPlayer).Losses++;
                    players.GetAt(dealer).Wins++;
                    message = players.GetAt(currentPlayer).ToString();
                }
                else if (players.GetAt(currentPlayer).Hand.Score >= players.GetAt(dealer).Hand.Score)
                {
                    players.GetAt(currentPlayer).IsFinnishied = true;
                    players.GetAt(currentPlayer).Winner = true;
                    players.GetAt(currentPlayer).Wins++;
                    players.GetAt(dealer).Losses++;
                    message = players.GetAt(currentPlayer).ToString();
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
     

        private void mnuXMLSerialize_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.ShowDialog();
            string saveFilePath = saveFile.FileName + ".xml";
            players.XMLSerialize(saveFilePath);
        }
    }
}

