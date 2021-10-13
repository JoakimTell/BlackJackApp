using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    class Hand
    {
        private Deck deck;
        private Card lastCard;
        private int numberOfCards;
        private int score;

        public Hand(Deck deck)
        {
            this.deck = deck;
        }

        #region Properties
        public Card LastCard
        {
            get { return lastCard; }
        }

        public int NumberOfCards
        {
            get { return numberOfCards; }
        }

        public int Score
        {
            get { return score; }
        }
        #endregion

        public void AddCard(Card card)
        {
            this.lastCard = card; // TODO: this correct? 
        }

        public void Clear()
        {
            lastCard = null;
            numberOfCards = 0;
            score = 0;
        }

        public override string ToString()
        {
            return $"Score: {score} \nNumber of cards: {numberOfCards} \nLast card: {lastCard}";
        }
    }
}
