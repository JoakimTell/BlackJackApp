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

        private int maxImageSlotsDealer; 
        private int maxImageSlotsPlayer;

        // Initialized without any game logic. the user has to start a new game.
        public GameWindow()
        {
            InitializeComponent();
            btnNextPlayer.IsEnabled = false;
            btnNewRound.IsEnabled = false;
        }

        #region BUTTON CLICKS
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
            deck.ToString(); // Debug to display shuffle effect.
        }

        private void btnNextPlayer_Click(object sender, RoutedEventArgs e)
        {
            game.NextMove();
        }
       
        private void btnNewRound_Click(object sender, RoutedEventArgs e)
        {
            btnNextPlayer.IsEnabled = true;
            if (players.Count > 0 && deck.GameIsDone)
            {
                game.CurrentPlayerPos = 0;
                UpdatePlayerListView();
                ClearGUI();
                ButtonsIsInPlaymode(false);

                deck.GameIsDone = false;
                foreach (Player player in players.List)
                {
                    player.IsFinnishied = false;
                }
                game.DealersFirstTwoCards();
            }
        }

        private void New_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            deck = new Deck(new List<Card>());
            deck.GameIsDone = true;
            players = new ListManager<Player>();
            game = new Game(players, deck);
            
            maxImageSlotsDealer = canvasDealerCards.Children.Count;
            maxImageSlotsPlayer = canvasDealerCards.Children.Count;

            // Go to Menu frame first.
            Menu menu = new Menu(game, players, deck);
            menu.Show();

            AddEventHandlers();
            // Initialize Game frame.
            ButtonsIsInPlaymode(false);
            btnNextPlayer.IsEnabled = false;
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
            game.CheckScore += OnCheckingScorePlayer;
            game.AfterEachMove += OnUpdateGUISituation;
            game.UpdateCards += (object sender, UpdateCardsEventArgs e) => RevealCardImage(e.Reveal, e.Dealer, e.CardPos, e.Card);
            game.AfterDealerMove += (object sender, string score) => { lblDealerScoreCalc.Content = score; };
        }

        // To update player info on events in BLL.
        private void OnCheckingScorePlayer(object sender, CheckScoreEventArgs e)
        {
            lblPlayerName.Content = e.PlayerName;
            lblPlayerScoreCalc.Content = e.PlayerScore;
        }

        // To update GUI info on events in BLL.
        private void OnUpdateGUISituation(object sender, EachMoveEventArgs e)
        {
            btnNextPlayer.Content = e.ButtonMessage;
            lblMessage.Content = e.LabelMessage;
            lblPlayerName.Content = e.PlayerNameMessage;
            lblPlayerScoreCalc.Content = e.PlayerScoreMessage;
            btnNextPlayer.IsEnabled = e.EnableButtonNextPlayer;
            ButtonsIsInPlaymode(e.EnableButtonsInPlayMode);
        }
        #endregion

        #region UPDATE GUI HELPER METHODS
        // Reset the table display.
        private void ClearGUI()
        {
            lblPlayerName.Content = "";
            lblPlayerScoreCalc.Content = "";
            lblMessage.Content = "Dealer is dealt first cards. Next player may start to play.";
            // Empty all card image slots.
            for (int cardPos = 0; cardPos < maxImageSlotsPlayer; cardPos++)
            {
                RevealCardImage(false, false, cardPos, null);
            }
            for (int cardPos = 0; cardPos < maxImageSlotsDealer; cardPos++)
            {
                RevealCardImage(false, true, cardPos, null);
            }
        }

        // Display a card image, back or front, on the image component, or clear the space.
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

        // Update the information in the player list.
        private void UpdatePlayerListView()
        {
            if (lstViewPlayerProgress.Items.Count == 0)
            {
                foreach (Player player in players.List)
                {
                    if (!(player.PlayerID == "DEALER"))
                    {
                        lstViewPlayerProgress.Items.Add(player);
                    }
                }
            }
            else
            {
                lstViewPlayerProgress.Items.Refresh();
            }
        }

        // Update Buttons in bulk, depending on whether a player is currently in a hand or not.
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