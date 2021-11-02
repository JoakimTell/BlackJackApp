using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilitiesLib;

namespace BlackJackApp
{
    public delegate void OnScoreCheckPlayerEventHandler(object source, EventArgs eventArgs);
    public delegate void OnScoreCheckButtonEventHandler(object source, bool enable);
    public delegate void OnScoreCheckMessageEventHandler(object source, string message);

    public class Game
    {
        private ListManager<Player> players;
        private Deck deck;

        public int CurrentPlayerPos { get; set; }
        private static int DealerPos { get; } = 0;

        public event OnScoreCheckPlayerEventHandler CheckingScorePlayer;
        public event OnScoreCheckButtonEventHandler CheckingScoreButton;
        public event OnScoreCheckMessageEventHandler CheckingScoreMessage;

        public Game(ListManager<Player> players, Deck deck)
        {
            this.players = players;
            this.deck = deck;
        }

        public void NextMove()
        {
            string message = "";

            Player dealer = players.GetAt(DealerPos);
            Player player = players.GetAt(CurrentPlayerPos);

            Hand dealerHand = dealer.Hand;
            Hand playerHand = player.Hand;

            int playerScore = playerHand.Score;
            int dealerScore = dealerHand.Score;

            if (!dealer.IsFinnishied) // Before dealers second turn, compare against dealers first card.
            {
                if (CurrentPlayerPos < players.Count)
                {
                    //for (int i = 0; i < 8; i++) // 8 in GUI is max slots for card images.
                    //{
                    //    Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
                    //    nextImage.Source = null;
                    //}
                    PlayerFirstTwoCards();
                    ScoreCheck();
                    if (CurrentPlayerPos == players.Count - 1)
                    {
                        //    btnNextPlayer.Content = "Dealers round";
                    }
                }
                else
                {
                    DealersSecondRound();
                    dealer.IsFinnishied = true;
                    CurrentPlayerPos = 0; // Reset position after every player is done.
                    bool score = dealerScore > 21;
                    //lblMessage.Content = score
                    //    ? "Dealer busts. Remaining players win. Compare scores."
                    //    : "Dealer stays. Compare scores.";
                    //lblPlayerName.Content = "";
                    //lblPlayerScoreCalc.Content = "";
                    for (int i = 0; i < 8; i++)
                    {
                        //    Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
                        //    nextImage.Source = null;
                    }
                    //btnNextPlayer.Content = "Compare score";
                }
            }
            else if (dealer.IsFinnishied) // Now compare players score to dealers final score.
            {
                if (CurrentPlayerPos < players.Count)
                {
                    for (int i = 0; i < playerHand.Cards.Count; i++)
                    {
                        //Image nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, i) as Image;
                        //nextImage.Source = RevealCard(players.GetAt(currentPlayer).Hand.Cards[i]);
                    }
                    ScoreCheck();
                    //ButtonsIsInPlaymode(false);
                    if (CurrentPlayerPos == players.Count - 1)
                    {
                        //btnNextPlayer.Content = "End Round";
                    }
                }
                else
                {
                    deck.GameIsDone = true;
                    //lblMessage.Content = "Round finnished. New game?";
                    //btnNextPlayer.Content = "Next Player";
                    //btnNextPlayer.IsEnabled = false;
                    foreach (Player p in players.List)
                    {
                        p.Hand.Clear();
                        p.IsFinnishied = true;
                    }
                }
            }
        }

        // Set up the current player for a hand.
        private void PlayerFirstTwoCards()
        {
            Player player = players.GetAt(CurrentPlayerPos);
            Hand hand = player.Hand;
            List<Card> twoCards = deck.GetTwoCards();
            //Image nextImage;

            // Reveal the players first two cards.
            foreach (Card c in twoCards)
            {
                hand.AddCard(c);
                int cardPos = twoCards.IndexOf(c);
                //nextImage = VisualTreeHelper.GetChild(canvasPlayerCards, cardPos) as Image;
                //nextImage.Source = RevealCard(hand.LastCard);
            }
        }

        // Dealer plays until score is between 15 to 21, or gets busted.
        private void DealersSecondRound()
        {
            Player dealer = players.GetAt(DealerPos);
            Hand hand = dealer.Hand;

            int first = 0;

            hand.AddCard(hand.LastCard);
            //dealerCard2.Source = RevealCard(hand.LastCard);

            // Add more cards.
            while (hand.Score < 15)
            {
                Card card = deck.GetAt(first);
                hand.AddCard(card);
                deck.RemoveCard(first);

                // Show a new card for the dealer.
                //Image nextImage = VisualTreeHelper.GetChild(canvasDealerCards, hand.NumberOfCards) as Image;
                //nextImage.Source = RevealCard(hand.LastCard);
            }

            //lblDealerScoreCalc.Content = hand.Score;
        }

        // Check player score and give message.
        public void ScoreCheck()
        {
            CheckingScorePlayer?.Invoke(this, EventArgs.Empty);  //lblPlayerName.Content = player.Name; lblPlayerScoreCalc.Content = playerScore;

            string message = "";

            Player dealer = players.GetAt(DealerPos);
            Player player = players.GetAt(CurrentPlayerPos);

            Hand dealerHand = dealer.Hand;
            Hand playerHand = player.Hand;

            int playerScore = playerHand.Score;
            int dealerScore = dealerHand.Score;

            if (!dealer.IsFinnishied)
            {
                if (playerScore == 21)
                {
                    player.Winner = true;
                    player.Wins++;
                    player.IsFinnishied = true; // To not draw more cards after dealer.
                    CheckingScoreButton?.Invoke(this, false); // ButtonsIsInPlaymode(false);
                    message = player.ToString();
                }
                else if (playerScore > 21)
                {
                    player.Winner = false;
                    player.Losses++;
                    player.IsFinnishied = true; // To not draw more cards after dealer.
                    CheckingScoreButton?.Invoke(this, false); //ButtonsIsInPlaymode(false);
                    message = player.ToString();
                }
                else
                {
                    if (playerScore < dealerScore)
                    {
                        message = "You have LESS points than the dealer.";
                    }
                    else if (playerScore > dealerScore)
                    {
                        message = "You have More points than the dealer.";
                    }
                    else if (playerScore == dealerScore)
                    {
                        message = "You have THE SAME score as the dealer.";
                    }
                    CheckingScoreButton?.Invoke(this, true); //ButtonsIsInPlaymode(true);
                }
            }
            else if(dealer.IsFinnishied)
            {
                if (player.IsFinnishied)
                {
                    message = player.ToString();
                }
                else if (dealerScore > 21)
                {
                    player.IsFinnishied = true;
                    player.Winner = true;
                    dealer.Losses++;
                    player.Wins++;
                    message = player.ToString();
                }
                else if (playerScore < dealerScore)
                {
                    player.IsFinnishied = true;
                    player.Winner = false;
                    player.Losses++;
                    dealer.Wins++;
                    message = player.ToString();
                }
                else if (playerScore >= dealerScore)
                {
                    player.IsFinnishied = true;
                    player.Winner = true;
                    player.Wins++;
                    dealer.Losses++;
                    message = player.ToString();
                }
                CheckingScoreButton?.Invoke(this, true); //ButtonsIsInPlaymode(true);
            }
            CheckingScoreMessage?.Invoke(this, message); //lblMessage.Content = message;
        }
    }
}
