using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilitiesLib;

namespace BlackJackApp
{
    public class Game
    {
        private ListManager<Player> players;
        private Deck deck;

        private int currentPlayerPos;
        private static int dealerPos = 0;

        public Game(ListManager<Player> players, Deck deck)
        {
            this.players = players;
            this.deck = deck;
        }

        // Check player score and give message.
        public void ScoreCheck()
        {
            //lblPlayerName.Content = player.Name;
            //lblPlayerScoreCalc.Content = playerScore;

            string message = "";

            Player dealer = players.GetAt(dealerPos);
            Player player = players.GetAt(currentPlayerPos);

            int playerScore = player.Hand.Score;
            int dealerScore = dealer.Hand.Score;

            if (!dealer.IsFinnishied)
            {
                if (playerScore == 21)
                {
                    player.Winner = true;
                    player.Wins++;
                    player.IsFinnishied = true; // To not draw more cards after dealer.
                    // ButtonsIsInPlaymode(false);
                    message = player.ToString();
                }
                else if (playerScore > 21)
                {
                    player.Winner = false;
                    player.Losses++;
                    player.IsFinnishied = true; // To not draw more cards after dealer.
                    //ButtonsIsInPlaymode(false);
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
                    //ButtonsIsInPlaymode(true);
                }
            }
            else
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
                //ButtonsIsInPlaymode(true);
            }
            //lblMessage.Content = message;
        }
    }
}
