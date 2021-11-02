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
        private Game game;

        private Deck deck;
        private ListManager<Player> players;

        private int currentPlayer;
        private static int dealerPos = 0;

        public GameWindow()
        {
            InitializeComponent();
            btnNextPlayer.IsEnabled = false;
            btnNewRound.IsEnabled = false;
        }

        #region IN GAME ACTION BUTTONS

        private void Hit_Button_Click(object sender, RoutedEventArgs e)
        {
            Hit();
        }

        private void Stay_Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonsIsInPlaymode(false);
        }

        private void Shuffle_Button_Click(object sender, RoutedEventArgs e)
        {
            deck.Shuffle();
            deck.ToString();
            // ShuffleCardsEffect();
        }

        private void btnNextPlayer_Click(object sender, RoutedEventArgs e)
        {
            currentPlayer++;
            game.CurrentPlayerPos++;
            btnNextPlayer.Content = "Next Player";

            //NextMove();
            game.NextMove();
        }
        #endregion

        #region OTHER BUTTONS
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

        private void New_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            deck = new Deck(new List<Card>());
            deck.GameIsDone = true;
            players = new ListManager<Player>();
            game = new Game(players, deck);
            Menu menu = new Menu(game, players, deck);
            menu.Show();
            ButtonsIsInPlaymode(false);
            btnNextPlayer.IsEnabled = false;
            game.CheckingScorePlayer += DisplayCurrentPlayer;
            game.CheckingScoreButton += (object sender, bool enabled) => ButtonsIsInPlaymode(enabled);
            game.CheckingScoreMessage += (object sender, string message) => { lblMessage.Content = message; };

            deck.DeckIsRunningOut += On_Deck_LowOnCards_GUI;
            deck.DeckIsRunningOut += (object sender, EventArgs e) =>
            { //Lambda statement
                Debug.WriteLine("GUI Lambda statement reached!");
                // Do something GUIy(object sender, EventArgs e) => On_Deck_LowOnCards_GUI
            };
            deck.DeckIsRunningOut += (object sender, EventArgs e) => LambdaExpression(sender, e);
        }

        private void mnuXMLSerialize_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.ShowDialog();
            string saveFilePath = saveFile.FileName + ".xml";
            players.XMLSerialize(saveFilePath);
        }
        #endregion

        #region SUBSCRIBE TO EVENTS
        private void On_Deck_LowOnCards_GUI(object sender, EventArgs e)
        {
            Debug.WriteLine("GUI method reached!");
            // ShuffleCardsEffect();
        }

        private void LambdaExpression(object sender, EventArgs e)
        {
            Debug.WriteLine("GUI Lambda expression reached!");
            // Do something GUIy again
        }

        private void DisplayCurrentPlayer(object sender, EventArgs e)
        {
            lblPlayerName.Content = players.GetAt(currentPlayer).Name;
            lblPlayerScoreCalc.Content = players.GetAt(currentPlayer).Hand.Score;
        }
        #endregion

        private void AddPlayersToListView()
        {
            if (lstViewPlayerProgress.Items.Count == 0)
            {
                foreach (Player player in players.List)
                {
                    if (!(player.PlayerID == "DEALER"))
                    {
                        var row = new { player.PlayerID, player.Wins, player.Losses };
                        lstViewPlayerProgress.Items.Add(player);
                    }
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

        private void Hit()
        {
            Player player = players.GetAt(currentPlayer);
            Hand hand = player.Hand;

            if (!player.IsFinnishied)
            {
                int first = 0;
                Card card = deck.GetAt(first);

                hand.AddCard(card);
                deck.RemoveCard(first);

                Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, hand.NumberOfCards) as Image;
                nextImage.Source = RevealCard(hand.LastCard);
            }
            //ScoreCheck();
            game.ScoreCheck();
        }

        // Dealer is dealt its first revealed card, and its hidden second card.
        private void DealersFirstTwoCards()
        {
            Player dealer = players.GetAt(dealerPos);
            Hand hand = dealer.Hand;

            List<Card> twoCards = deck.GetTwoCards();
            int first = 0;
            int second = 1;

            // Reaveal first added card.
            hand.AddCard(twoCards[first]);
            dealerCard1.Source = RevealCard(hand.LastCard);

            // Hide second added card.
            hand.LastCard = twoCards[second];
            dealerCard2.Source = HideCard();

            lblDealerScoreCalc.Content = hand.Score;
        }



        #region IMAGE SETTERS
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
        #endregion

        #region BUTTON ENABLING
        private void ButtonsIsInPlaymode(bool playing)
        {
            btnHit.IsEnabled = playing;
            btnStay.IsEnabled = playing;
            btnNextPlayer.IsEnabled = !playing;
            btnNewRound.IsEnabled = !playing;
        }
        #endregion

        #region OBSOLETE METHODS...
        // Check player score and give message.
        //private void ScoreCheck()
        //{
        //    lblPlayerName.Content = players.GetAt(currentPlayer).Name;
        //    lblPlayerScoreCalc.Content = players.GetAt(currentPlayer).Hand.Score;

        //    string message = "";

        //    if (!players.GetAt(dealerPos).IsFinnishied)
        //    {
        //        if (players.GetAt(currentPlayer).Hand.Score == 21)
        //        {
        //            players.GetAt(currentPlayer).Winner = true;
        //            players.GetAt(currentPlayer).Wins++;
        //            players.GetAt(currentPlayer).IsFinnishied = true; // To not draw more cards after dealer.
        //            ButtonsIsInPlaymode(false);
        //            message = players.GetAt(currentPlayer).ToString();
        //        }
        //        else if (players.GetAt(currentPlayer).Hand.Score > 21)
        //        {
        //            players.GetAt(currentPlayer).Winner = false;
        //            players.GetAt(currentPlayer).Losses++;
        //            players.GetAt(currentPlayer).IsFinnishied = true; // To not draw more cards after dealer.
        //            ButtonsIsInPlaymode(false);
        //            message = players.GetAt(currentPlayer).ToString();
        //        }
        //        else
        //        {
        //            if (players.GetAt(currentPlayer).Hand.Score < players.GetAt(dealerPos).Hand.Score)
        //            {
        //                message = "You have LESS points than the dealer.";
        //            }
        //            else if (players.GetAt(currentPlayer).Hand.Score > players.GetAt(dealerPos).Hand.Score)
        //            {
        //                message = "You have More points than the dealer.";
        //            }
        //            else if (players.GetAt(currentPlayer).Hand.Score == players.GetAt(dealerPos).Hand.Score)
        //            {
        //                message = "You have THE SAME score as the dealer.";
        //            }
        //            ButtonsIsInPlaymode(true);
        //        }
        //    }
        //    else
        //    {
        //        if (players.GetAt(currentPlayer).IsFinnishied)
        //        {
        //            message = players.GetAt(currentPlayer).ToString();
        //        }
        //        else if (players.GetAt(dealerPos).Hand.Score > 21)
        //        {
        //            players.GetAt(currentPlayer).IsFinnishied = true;
        //            players.GetAt(currentPlayer).Winner = true;
        //            players.GetAt(dealerPos).Losses++;
        //            players.GetAt(currentPlayer).Wins++;
        //            message = players.GetAt(currentPlayer).ToString();
        //        }
        //        else if (players.GetAt(currentPlayer).Hand.Score < players.GetAt(dealerPos).Hand.Score)
        //        {
        //            players.GetAt(currentPlayer).IsFinnishied = true;
        //            players.GetAt(currentPlayer).Winner = false;
        //            players.GetAt(currentPlayer).Losses++;
        //            players.GetAt(dealerPos).Wins++;
        //            message = players.GetAt(currentPlayer).ToString();
        //        }
        //        else if (players.GetAt(currentPlayer).Hand.Score >= players.GetAt(dealerPos).Hand.Score)
        //        {
        //            players.GetAt(currentPlayer).IsFinnishied = true;
        //            players.GetAt(currentPlayer).Winner = true;
        //            players.GetAt(currentPlayer).Wins++;
        //            players.GetAt(dealerPos).Losses++;
        //            message = players.GetAt(currentPlayer).ToString();
        //        }
        //        ButtonsIsInPlaymode(true);
        //    }
        //    lblMessage.Content = message;
        //}
        #endregion

        //private void NextMove()
        //{
        //    if (!players.GetAt(dealerPos).IsFinnishied) // Before dealers second turn, compare against dealers first card.
        //    {
        //        if (currentPlayer < players.Count)
        //        {
        //            for (int i = 0; i < 8; i++)
        //            {
        //                Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
        //                nextImage.Source = null;
        //            }
        //            PlayerFirstTwoCards();
        //            //ScoreCheck();
        //            game.ScoreCheck();
        //            if (currentPlayer == players.Count - 1)
        //            {
        //                btnNextPlayer.Content = "Dealers round";
        //            }
        //        }
        //        else
        //        {
        //            DealersSecondRound();
        //            players.GetAt(dealerPos).IsFinnishied = true;
        //            currentPlayer = 0;
        //            lblMessage.Content = players.GetAt(dealerPos).Hand.Score > 21
        //                ? "Dealer busts. Remaining players win. Compare scores."
        //                : "Dealer stays. Compare scores.";
        //            lblPlayerName.Content = "";
        //            lblPlayerScoreCalc.Content = "";
        //            for (int i = 0; i < 8; i++)
        //            {
        //                Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
        //                nextImage.Source = null;
        //            }
        //            btnNextPlayer.Content = "Compare score";
        //        }
        //    }
        //    else // Now compare players score to dealers final score.
        //    {
        //        if (currentPlayer < players.Count)
        //        {
        //            for (int i = 0; i < players.GetAt(currentPlayer).Hand.Cards.Count; i++)
        //            {
        //                Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
        //                nextImage.Source = RevealCard(players.GetAt(currentPlayer).Hand.Cards[i]);
        //            }
        //            //ScoreCheck();
        //            game.ScoreCheck();
        //            ButtonsIsInPlaymode(false);
        //            if (currentPlayer == players.Count - 1)
        //            {
        //                btnNextPlayer.Content = "End Round";
        //            }
        //        }
        //        else
        //        {
        //            deck.GameIsDone = true;
        //            lblMessage.Content = "Round finnished. New game?";
        //            btnNextPlayer.Content = "Next Player";
        //            btnNextPlayer.IsEnabled = false;
        //            foreach (Player player in players.List)
        //            {
        //                player.Hand.Clear();
        //                player.IsFinnishied = true;
        //            }
        //        }
        //    }
        //}

        // Set up the current player for a hand.
        //private void PlayerFirstTwoCards()
        //{
        //    Hand hand = players.GetAt(currentPlayer).Hand;
        //    List<Card> playerFirstTwoCards = deck.GetTwoCards();
        //    Image nextImage;

        //    // Reveal the players first two cards.
        //    for (int i = 0; i < 2; i++)
        //    {
        //        hand.AddCard(playerFirstTwoCards[i]);
        //        nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
        //        nextImage.Source = RevealCard(hand.LastCard);
        //    }
        //}

        // Dealer plays until score is between 15 to 21, or gets busted.
        //private void DealersSecondRound()
        //{
        //    Hand hand = players.GetAt(dealerPos).Hand;

        //    // Reaveal second added card.
        //    hand.AddCard(hand.LastCard);
        //    dealerCard2.Source = RevealCard(hand.LastCard);

        //    // Add more cards.
        //    while (hand.Score < 15)
        //    {
        //        Card card = deck.GetAt(0);
        //        hand.AddCard(card);
        //        deck.RemoveCard(0);

        //        // Show a new card for the dealer.
        //        Image nextImage = VisualTreeHelper.GetChild(canvasDealerCards, hand.NumberOfCards) as Image;
        //        nextImage.Source = RevealCard(hand.LastCard);
        //    }

        //    lblDealerScoreCalc.Content = hand.Score;
        //}
    }
}

