using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilitiesLib;

namespace BlackJackApp
{
    #region EVENTHANDLING
    public delegate void OnCheckScoreEventHandler(object source, CheckScoreEventArgs e);
    public delegate void AfterDealerMoveEventHandler(object source, string score);
    public delegate void OnUpdateCardsEventHandler(object source, UpdateCardsEventArgs eArgs);
    public delegate void AfterEachMoveEventHandler(object source, EachMoveEventArgs e);

    public class UpdateCardsEventArgs : EventArgs
    {
        public UpdateCardsEventArgs(bool reveal, bool dealer, int cardPos, Card card)
        {
            Reveal = reveal;
            Dealer = dealer;
            CardPos = cardPos;
            Card = card;
        }

        public bool Reveal { get; set; }
        public bool Dealer { get; set; }
        public int CardPos { get; set; }
        public Card Card { get; set; }
    }

    public class EachMoveEventArgs : EventArgs
    {
        public EachMoveEventArgs(string buttonMessage, string labelMessage, string playerNameMessage, string playerScoreMessage, bool enableButtonNextPlayer, bool enableButtonsInPlayMode)
        {
            ButtonMessage = buttonMessage;
            LabelMessage = labelMessage;
            PlayerNameMessage = playerNameMessage;
            PlayerScoreMessage = playerScoreMessage;
            EnableButtonNextPlayer = enableButtonNextPlayer;
            EnableButtonsInPlayMode = enableButtonsInPlayMode;
        }

        public string ButtonMessage { get; set; }
        public string LabelMessage { get; set; }
        public string PlayerNameMessage { get; set; }
        public string PlayerScoreMessage { get; set; }
        public bool EnableButtonNextPlayer { get; set; }
        public bool EnableButtonsInPlayMode { get; set; }
    }

    public class CheckScoreEventArgs : EventArgs
    {
        public CheckScoreEventArgs(string playerName, string playerScore, bool handInPlay, string scoreInfo)
        {
            PlayerName = playerName;
            PlayerScore = playerScore;
            HandInPlay = handInPlay;
            ScoreInfo = scoreInfo;
        }

        public string PlayerName { get; set; }
        public string PlayerScore { get; set; }
        public bool HandInPlay { get; set; }
        public string ScoreInfo { get; set; }
    }
    #endregion

    public class Game
    {
        private ListManager<Player> players;
        private Deck deck;

        public int CurrentPlayerPos { get; set; }
        private int DealerPos { get; } = 0;
        private readonly int maxImageSlots = 8; 

        public event OnCheckScoreEventHandler CheckScore;
        public event AfterDealerMoveEventHandler AfterDealerMove;
        public event OnUpdateCardsEventHandler UpdateCards;
        public event AfterEachMoveEventHandler AfterEachMove;

        public Game(ListManager<Player> players, Deck deck)
        {
            this.players = players;
            this.deck = deck;
        }

        public void NextMove()
        {
            CurrentPlayerPos++;

            string buttonMessage = "";
            string labelMessage = "";
            string playerNameMessage = "";
            string playerScoreMessage = "";

            bool enableButtonNextPlayer = false;
            bool enableButtonsInPlayMode = false;

            Player dealer = players.GetAt(DealerPos);
            Player player = players.GetAt(CurrentPlayerPos);
            //Debug.WriteLine("Game.NextMove() - Current Player Position: " + CurrentPlayerPos);

            Hand dealerHand = dealer.Hand;

            int dealerScore = dealerHand.Score;

            if (!dealer.IsFinnishied) // Before dealers second turn, compare against dealers first card.
            {
                //Debug.WriteLine("if (!dealer.IsFinnishied): " + CurrentPlayerPos);
                if (CurrentPlayerPos < players.Count)
                {
                    //Debug.WriteLine("if (CurrentPlayerPos < players.Count): " + CurrentPlayerPos);
                    for (int cardPos = 0; cardPos < 8; cardPos++) // 8 in GUI is max slots for card images.
                    {
                        UpdateCards?.Invoke(this, new UpdateCardsEventArgs(true, false, cardPos, null));
                    }
                    PlayerFirstTwoCards();
                    playerNameMessage = player.Name;
                    playerScoreMessage = player.Hand.Score.ToString();
                    //Debug.WriteLine("");
                    //Debug.WriteLine("Game.NextMove() - Player: " + player.Name + ", Score: " + playerScoreMessage);
                    //Debug.WriteLine("");
                    enableButtonsInPlayMode = true;
                    buttonMessage = "Next Player";
                    if (CurrentPlayerPos == players.Count - 1)
                    {
                        buttonMessage = "Dealers round";
                    }
                    AfterEachMove?.Invoke(this, new EachMoveEventArgs(buttonMessage, labelMessage, playerNameMessage, playerScoreMessage, enableButtonNextPlayer, enableButtonsInPlayMode));
                    ScoreCheck();
                }
                else
                {
                    //Debug.WriteLine("1 else: " + CurrentPlayerPos);
                    DealersSecondRound();
                    dealer.IsFinnishied = true;
                    CurrentPlayerPos = 0; // Reset position after every player is done.
                    bool score = dealerScore > 21;
                    labelMessage = score
                        ? "Dealer busts. Remaining players win. Compare scores."
                        : "Dealer stays. Compare scores.";
                    playerNameMessage = "";
                    playerScoreMessage = "";
                    enableButtonNextPlayer = true;
                    for (int cardPos = 0; cardPos < maxImageSlots; cardPos++)
                    {
                        UpdateCards?.Invoke(this, new UpdateCardsEventArgs(true, false, cardPos, null));
                    }
                    buttonMessage = "Compare score";
                    AfterEachMove?.Invoke(this, new EachMoveEventArgs(buttonMessage, labelMessage, playerNameMessage, playerScoreMessage, true, false));
                }
            }
            else if (dealer.IsFinnishied) // Now compare players score to dealers final score.
            {
                //Debug.WriteLine("else if (dealer.IsFinnishied): " + CurrentPlayerPos);
                if (CurrentPlayerPos < players.Count)
                {
                    //Debug.WriteLine("if (CurrentPlayerPos < players.Count): " + CurrentPlayerPos);
                    buttonMessage = "Next Player";
                    labelMessage = player.ToString();
                    playerNameMessage = player.Name;
                    playerScoreMessage = player.Hand.Score.ToString();
                    for (int cardPos = 0; cardPos < player.Hand.Cards.Count; cardPos++)
                    {
                        Card card = player.Hand.Cards[cardPos];                        
                        UpdateCards?.Invoke(this, new UpdateCardsEventArgs(true, false, cardPos, card));
                    }
                    if (CurrentPlayerPos == players.Count - 1)
                    {
                        buttonMessage = "End Round";
                    }
                    //ScoreCheck();
                    AfterEachMove?.Invoke(this, new EachMoveEventArgs(buttonMessage, labelMessage, playerNameMessage, playerScoreMessage, true, false));
                }
                else
                {
                    //Debug.WriteLine("2 else: " + CurrentPlayerPos);
                    if (CurrentPlayerPos < players.Count)
                    deck.GameIsDone = true;
                    labelMessage = "Round finnished. New game?";
                    buttonMessage = "Next Player";
                    enableButtonNextPlayer = false;
                    CurrentPlayerPos = 0; // Reset position after every player is done.
                    foreach (Player p in players.List)
                    {
                        p.Hand.Clear();
                        p.IsFinnishied = true;
                    }
                    for (int cardPos = 0; cardPos < maxImageSlots; cardPos++) // 8 in GUI is max slots for card images.
                    {
                        UpdateCards?.Invoke(this, new UpdateCardsEventArgs(false, false, cardPos, null));
                    }
                    AfterEachMove?.Invoke(this, new EachMoveEventArgs(buttonMessage, labelMessage, playerNameMessage, playerScoreMessage, enableButtonNextPlayer, enableButtonsInPlayMode));
                }
            }
        }

        // Dealer is dealt its first revealed card, and its hidden second card.
        public void DealersFirstTwoCards()
        {
            Player dealer = players.GetAt(DealerPos);
            Hand hand = dealer.Hand;

            List<Card> twoCards = deck.GetTwoCards();
            int first = 0;
            int second = 1;

            // Reaveal first added card.
            Card card = twoCards[first];
            hand.AddCard(card);
            UpdateCards?.Invoke(this, new UpdateCardsEventArgs(true, true, first, card));

            // Hide second added card.
            card = twoCards[second];
            hand.LastCard = card; // Store second card in memory to add and reveal later, after players round.
            UpdateCards?.Invoke(this, new UpdateCardsEventArgs(false, true, second, card));

            string score = hand.Score.ToString();
            AfterDealerMove?.Invoke(this, score);
        }

        // Set up the current player for a hand.
        private void PlayerFirstTwoCards()
        {
            Player player = players.GetAt(CurrentPlayerPos);
            Hand hand = player.Hand;

            // Reveal the players first two cards.
            List<Card> twoCards = deck.GetTwoCards();
            foreach (Card card in twoCards)
            {
                hand.AddCard(card);
                int cardPos = twoCards.IndexOf(card);
                UpdateCards?.Invoke(this, new UpdateCardsEventArgs(true, false, cardPos, card));
            }
        }

        // Dealer plays until score is between 15 to 21, or gets busted.
        private void DealersSecondRound()
        {
            Player dealer = players.GetAt(DealerPos);
            Hand hand = dealer.Hand;

            int dealerScore = hand.Score;

            int first = 0;
            int second = 1;

            Card card = hand.LastCard;

            hand.AddCard(card);
            int cardPos = second;
            UpdateCards?.Invoke(this, new UpdateCardsEventArgs(true, true, cardPos, card));

            // Add more cards.
            while (hand.Score < 15)
            {
                card = deck.GetAt(first);
                hand.AddCard(card);
                deck.RemoveCard(first);
                cardPos = hand.NumberOfCards - 1;
                UpdateCards?.Invoke(this, new UpdateCardsEventArgs(true, true, cardPos, card));
            }
            dealerScore = hand.Score;
            AfterDealerMove?.Invoke(this, dealerScore.ToString());
        }

        // Player takes a new card.
        public void Hit()
        {
            Player player = players.GetAt(CurrentPlayerPos);
            Hand hand = player.Hand;

            if (!player.IsFinnishied)
            {
                int first = 0;
                Card card = deck.GetAt(first);

                hand.AddCard(card);
                deck.RemoveCard(first);

                int cardPos = hand.NumberOfCards - 1;
                UpdateCards?.Invoke(this, new UpdateCardsEventArgs(true, false, cardPos, card));
            }
            ScoreCheck();
        }

        // Check player score and give message.
        public void ScoreCheck()
        {
            string scoreInfo = "";

            Player dealer = players.GetAt(DealerPos);
            Player player = players.GetAt(CurrentPlayerPos);

            Hand dealerHand = dealer.Hand;
            Hand playerHand = player.Hand;

            int playerScore = playerHand.Score;
            int dealerScore = dealerHand.Score;

            bool handInPlay = false;

            if (!dealer.IsFinnishied)
            {
                if (playerScore == 21)
                {
                    player.Winner = true;
                    player.Wins++;
                    player.IsFinnishied = true; // To not draw more cards after dealer.
                    handInPlay = false;
                    scoreInfo = player.ToString();
                }
                else if (playerScore > 21)
                {
                    player.Winner = false;
                    player.Losses++;
                    player.IsFinnishied = true; // To not draw more cards after dealer.
                    handInPlay = false;
                    scoreInfo = player.ToString();
                }
                else
                {
                    if (playerScore < dealerScore)
                    {
                        scoreInfo = "You have LESS points than the dealer.";
                    }
                    else if (playerScore > dealerScore)
                    {
                        scoreInfo = "You have More points than the dealer.";
                    }
                    else if (playerScore == dealerScore)
                    {
                        scoreInfo = "You have THE SAME score as the dealer.";
                    }
                    handInPlay = true;
                }
            }
            else if (dealer.IsFinnishied)
            {
                if (player.IsFinnishied)
                {
                    scoreInfo = player.ToString();
                }
                else if (dealerScore > 21)
                {
                    player.IsFinnishied = true;
                    player.Winner = true;
                    dealer.Losses++;
                    player.Wins++;
                    scoreInfo = player.ToString();
                }
                else if (playerScore < dealerScore)
                {
                    player.IsFinnishied = true;
                    player.Winner = false;
                    player.Losses++;
                    dealer.Wins++;
                    scoreInfo = player.ToString();
                }
                else if (playerScore >= dealerScore)
                {
                    player.IsFinnishied = true;
                    player.Winner = true;
                    player.Wins++;
                    dealer.Losses++;
                    scoreInfo = player.ToString();
                }
                handInPlay = true;
            }
            string name = player.Name;
            string score = player.Hand.Score.ToString();
            CheckScore?.Invoke(this, new CheckScoreEventArgs(name, score, handInPlay, scoreInfo));
        }
    }
}
