using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilitiesLib;

namespace BlackJackApp
{
    public delegate void OnScoreCheckPlayerEventHandler(object source, UpdatePlayerSituationEventArgs e);
    public delegate void OnScoreCheckButtonEventHandler(object source, bool enable);
    public delegate void OnScoreCheckMessageEventHandler(object source, string message);
    public delegate void OnNextMoveImageEventHandler(object source, UpdateCardsSituationEventArgs eArgs);
    public delegate void OnUpdateMessageEventHandler(object source, string message);
    public delegate void OnUpdateGUIEventHandler(object source, UpdateGUISituationEventArgs e);

    public class UpdateCardsSituationEventArgs : EventArgs
    {
        public bool Reveal { get; set; }
        public bool Dealer { get; set; }
        public int CardPos { get; set; }
        public Card Card { get; set; }
    }

    public class UpdateGUISituationEventArgs : EventArgs
    {
        public string ButtonMessage { get; set; }
        public string LabelMessage { get; set; }
        public string PlayerNameMessage { get; set; }
        public string PlayerScoreMessage { get; set; }
        public bool EnableButtonNextPlayer { get; set; }
        public bool EnableButtonsInPlayMode { get; set; }
    }

    public class UpdatePlayerSituationEventArgs : EventArgs
    {
        public string PlayerNameMessage { get; set; }
        public string PlayerScoreMessage { get; set; }
    }

    public class Game
    {
        private ListManager<Player> players;
        private Deck deck;

        public int CurrentPlayerPos { get; set; }
        private static int DealerPos { get; } = 0;
        private static int maxImageSlots = 8; // canvasDealerCards and canvasPlayerCards are equal.

        public event OnScoreCheckPlayerEventHandler CheckingScorePlayer;
        public event OnScoreCheckButtonEventHandler CheckingScoreButton;
        public event OnScoreCheckMessageEventHandler CheckingScoreMessage;
        public event OnNextMoveImageEventHandler ImageUpdateSituation;
        public event OnUpdateMessageEventHandler MessageUpdateSituation;
        public event OnUpdateGUIEventHandler UpdateGUISituation;

        public Game(ListManager<Player> players, Deck deck)
        {
            this.players = players;
            this.deck = deck;
        }

        public void NextMove()
        {
            string buttonMessage = "";
            string labelMessage = "";
            string playerNameMessage = "";
            string playerScoreMessage = "";

            bool enableButtonNextPlayer = false;
            bool enableButtonsInPlayMode = false;

            Player dealer = players.GetAt(DealerPos);
            Player player = players.GetAt(CurrentPlayerPos);
            Debug.WriteLine("");
            Debug.WriteLine("Game.NextMove() - Current Player Position: " + CurrentPlayerPos);
            Debug.WriteLine("");

            Hand dealerHand = dealer.Hand;

            int dealerScore = dealerHand.Score;

            if (!dealer.IsFinnishied) // Before dealers second turn, compare against dealers first card.
            {
                if (CurrentPlayerPos < players.Count)
                {
                    for (int cardPos = 0; cardPos < 8; cardPos++) // 8 in GUI is max slots for card images.
                    {
                        UpdateCardsSituationEventArgs args = new UpdateCardsSituationEventArgs();
                        args.Reveal = true;
                        args.Dealer = false;
                        args.CardPos = cardPos;
                        args.Card = null;
                        ImageUpdateSituation?.Invoke(this, args);
                    }
                    PlayerFirstTwoCards();
                    ScoreCheck();
                    playerNameMessage = player.Name;
                    playerScoreMessage = player.Hand.Score.ToString();
                    Debug.WriteLine("");
                    Debug.WriteLine("Game.NextMove() - Player: " + player.Name + ", Score: " + playerScoreMessage);
                    Debug.WriteLine("");
                    enableButtonsInPlayMode = true;
                    buttonMessage = "Next Player";
                    if (CurrentPlayerPos == players.Count - 1)
                    {
                        buttonMessage = "Dealers round";
                    }
                }
                else
                {
                    DealersSecondRound();
                    dealer.IsFinnishied = true;
                    CurrentPlayerPos = 0; // Reset position after every player is done.
                    bool score = dealerScore > 21;
                    labelMessage = score
                        ? "Dealer busts. Remaining players win. Compare scores."
                        : "Dealer stays. Compare scores.";
                    playerNameMessage = "";
                    playerScoreMessage = "";
                    for (int cardPos = 0; cardPos < maxImageSlots; cardPos++)
                    {
                        UpdateCardsSituationEventArgs args = new UpdateCardsSituationEventArgs();
                        args.Reveal = true;
                        args.Dealer = false;
                        args.CardPos = cardPos;
                        args.Card = null;
                        ImageUpdateSituation?.Invoke(this, args);
                    }
                    buttonMessage = "Compare score";
                }
            }
            else if (dealer.IsFinnishied) // Now compare players score to dealers final score.
            {
                if (CurrentPlayerPos < players.Count)
                {
                    for (int cardPos = 0; cardPos < player.Hand.Cards.Count; cardPos++)
                    {
                        Card card = player.Hand.Cards[cardPos];
                        
                        UpdateCardsSituationEventArgs args = new UpdateCardsSituationEventArgs();
                        args.Reveal = true;
                        args.Dealer = false;
                        args.CardPos = cardPos;
                        args.Card = card;
                        ImageUpdateSituation?.Invoke(this, args);
                    }
                    ScoreCheck();
                    enableButtonsInPlayMode = false;
                    if (CurrentPlayerPos == players.Count - 1)
                    {
                        buttonMessage = "End Round";
                    }
                }
                else
                {
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
                        UpdateCardsSituationEventArgs args = new UpdateCardsSituationEventArgs();
                        args.Reveal = false;
                        args.Dealer = false;
                        args.CardPos = cardPos;
                        args.Card = null;
                        ImageUpdateSituation?.Invoke(this, args);
                    }
                }
            }
            UpdateGUISituationEventArgs argsGUI = new UpdateGUISituationEventArgs();
            argsGUI.ButtonMessage = buttonMessage;
            argsGUI.LabelMessage = labelMessage;
            argsGUI.PlayerNameMessage = playerNameMessage;
            argsGUI.PlayerScoreMessage = playerScoreMessage;
            argsGUI.EnableButtonNextPlayer = enableButtonNextPlayer;
            argsGUI.EnableButtonsInPlayMode = enableButtonsInPlayMode;
            UpdateGUISituation?.Invoke(this, argsGUI);
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
                UpdateCardsSituationEventArgs args = new UpdateCardsSituationEventArgs();
                args.Reveal = true;
                args.Dealer = false;
                args.CardPos = cardPos;
                args.Card = card;
                ImageUpdateSituation?.Invoke(this, args);
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
            UpdateCardsSituationEventArgs args = new UpdateCardsSituationEventArgs();
            args.Reveal = true;
            args.Dealer = true;
            args.CardPos = cardPos;
            args.Card = card;
            ImageUpdateSituation?.Invoke(this, args);

            // Add more cards.
            while (hand.Score < 15)
            {
                card = deck.GetAt(first);
                hand.AddCard(card);
                deck.RemoveCard(first);
                cardPos = hand.NumberOfCards - 1;
                args = new UpdateCardsSituationEventArgs();
                args.Reveal = true;
                args.Dealer = true;
                args.CardPos = cardPos;
                args.Card = card;
                ImageUpdateSituation?.Invoke(this, args);
            }
            dealerScore = hand.Score;
            MessageUpdateSituation?.Invoke(this, dealerScore.ToString());
        }

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
                UpdateCardsSituationEventArgs args = new UpdateCardsSituationEventArgs();
                args.Reveal = true;
                args.Dealer = false;
                args.CardPos = cardPos;
                args.Card = card;
                ImageUpdateSituation?.Invoke(this, args);

            }
            ScoreCheck();
        }

        // Check player score and give message.
        public void ScoreCheck()
        {
            string message = "";

            Player dealer = players.GetAt(DealerPos);
            Player player = players.GetAt(CurrentPlayerPos);

            Hand dealerHand = dealer.Hand;
            Hand playerHand = player.Hand;

            int playerScore = playerHand.Score;
            int dealerScore = dealerHand.Score;

            bool enableButtonsInPlayMode = false;

            if (!dealer.IsFinnishied)
            {
                if (playerScore == 21)
                {
                    player.Winner = true;
                    player.Wins++;
                    player.IsFinnishied = true; // To not draw more cards after dealer.
                    enableButtonsInPlayMode = false;
                    message = player.ToString();
                }
                else if (playerScore > 21)
                {
                    player.Winner = false;
                    player.Losses++;
                    player.IsFinnishied = true; // To not draw more cards after dealer.
                    enableButtonsInPlayMode = false;
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
                    enableButtonsInPlayMode = true;
                }
            }
            else if (dealer.IsFinnishied)
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
                enableButtonsInPlayMode = true;
            }
            UpdatePlayerSituationEventArgs argsPlayer = new UpdatePlayerSituationEventArgs();
            argsPlayer.PlayerNameMessage = player.Name;
            argsPlayer.PlayerScoreMessage = player.Hand.Score.ToString();
            CheckingScorePlayer?.Invoke(this, argsPlayer);
            CheckingScoreButton?.Invoke(this, enableButtonsInPlayMode); // ButtonsIsInPlaymode(false);
            CheckingScoreMessage?.Invoke(this, message); //lblMessage.Content = message;
        }
    }
}
