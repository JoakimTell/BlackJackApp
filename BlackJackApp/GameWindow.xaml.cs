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
        private static int maxImageSlots = 8; // canvasDealerCards and canvasPlayerCards are equal.

        // Initialized without any game logic. the user has to start a new game.
        public GameWindow()
        {
            InitializeComponent();
            btnNextPlayer.IsEnabled = false;
            btnNewRound.IsEnabled = false;
        }

        #region IN GAME ACTION BUTTONS

        private void Hit_Button_Click(object sender, RoutedEventArgs e)
        {
            game.Hit();
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
                    game.CurrentPlayerPos = 0;
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
            StartUpBusinessLogic();
            // Go to Menu frame first.
            Menu menu = new Menu(game, players, deck);
            menu.Show();
            // Initialize Game frame.
            ButtonsIsInPlaymode(false);
            btnNextPlayer.IsEnabled = false;
            AddEventHandlers();
        }

        private void mnuXMLSerialize_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.ShowDialog();
            string saveFilePath = saveFile.FileName + ".xml";
            players.XMLSerialize(saveFilePath);
        }
        #endregion

        #region EVENTS HANDLERS
        // Subscribe to events from a publisher from the business logic layer.
        private void AddEventHandlers()
        {
            game.CheckingScorePlayer += OnCheckingScorePlayer;
            game.CheckingScoreButton += (object sender, bool enabled) => ButtonsIsInPlaymode(enabled);
            game.CheckingScoreMessage += (object sender, string message) => { lblMessage.Content = message; };
            game.ImageUpdateSituation += (object sender, UpdateCardsSituationEventArgs e) => RevealCardImage(e.Reveal, e.Dealer, e.CardPos, e.Card);
            game.UpdateGUISituation += OnUpdateGUISituation;
            game.MessageUpdateSituation += (object sender, string message) => { lblDealerScoreCalc.Content = message; };
        }

        // To update player info on events in BLL.
        private void OnCheckingScorePlayer(object sender, UpdatePlayerSituationEventArgs e)
        {
            lblPlayerName.Content = e.PlayerNameMessage;
            lblPlayerScoreCalc.Content = e.PlayerScoreMessage;
        }

        // To update GUI info on events in BLL.
        private void OnUpdateGUISituation(object sender, UpdateGUISituationEventArgs e)
        {
            btnNextPlayer.Content = e.ButtonMessage;
            lblMessage.Content = e.LabelMessage;
            lblPlayerName.Content = e.PlayerNameMessage;
            lblPlayerScoreCalc.Content = e.PlayerScoreMessage;

            btnNextPlayer.IsEnabled = e.EnableButtonNextPlayer;
            ButtonsIsInPlaymode(e.EnableButtonsInPlayMode);
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

        private void StartUpBusinessLogic()
        {
            deck = new Deck(new List<Card>());
            deck.GameIsDone = true;
            players = new ListManager<Player>();
            game = new Game(players, deck);
        }

        private void StartNewRound()
        {
            AddPlayersToListView();
            lblPlayerName.Content = "";
            lblPlayerScoreCalc.Content = "";
            lblMessage.Content = "Dealer is dealt first cards. Next player may start to play.";
            // Empty all card image slots.
            for (int cardPos = 0; cardPos < maxImageSlots; cardPos++)
            {
                RevealCardImage(false, false, cardPos, null);
                RevealCardImage(false, true, cardPos, null);
            }

            deck.GameIsDone = false;
            foreach (Player player in players.List)
            {
                player.IsFinnishied = false;
            }
            DealersFirstTwoCards();

            ButtonsIsInPlaymode(false);
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
            Card card = twoCards[first];
            hand.AddCard(card);
            RevealCardImage(true, true, first, card);

            // Hide second added card.
            card = twoCards[second];
            hand.LastCard = card; // Store second card in memory to add and reveal later, after players round.
            RevealCardImage(false, true, second, card);

            lblDealerScoreCalc.Content = hand.Score;
        }

        #region IMAGE SETTERS
        private void RevealCardImage(bool reveal, bool dealer, int cardPos, Card card)
        {
            // Which image slot to use.
            Image nextImage;
            if (dealer)
            {
                nextImage = VisualTreeHelper.GetChild(canvasDealerCards, cardPos) as Image;
            }
            else
            {
                nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, cardPos) as Image;
            }
            // Which image file to use.
            BitmapImage bitmapImage = null;
            if (card != null)
            {
                if (reveal)
                {
                    Value value = card.Value;
                    Suit suit = card.Suit;
                    string projectPathLocal = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName; //TODO: folder depth not universal
                    string path = System.IO.Path.Combine(projectPathLocal, @"Assets\Cards\", $"{value}of{suit}.png");
                    bitmapImage = new BitmapImage(new Uri(path));
                }
                else
                {
                    string projectPathLocal = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName; //TODO: folder depth not universal
                    string path = System.IO.Path.Combine(projectPathLocal, @"Assets\Cards\", $"cardBack_blue.png");
                    bitmapImage = new BitmapImage(new Uri(path));
                }
            }
            nextImage.Source = bitmapImage;
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
    }
}

