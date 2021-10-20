using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    public class Hand
    {
        private Deck deck; // All hands uses the same deck.
        
        #region Constructors
        public Hand(Deck deck) // All hands uses the same deck.
        {
            this.deck = deck;
        }
        #endregion

        #region Properties
        public Card LastCard { get; private set; }

        public int NumberOfCards { get; private set; }

        public int Score { get; private set; }
        #endregion

        public void AddCard(Card card)
        {
            LastCard = card;
            Score += (int) card.Value;
        }

        public void Clear()
        {
            LastCard = null;
            NumberOfCards = 0;
            Score = 0;
        }

        public override string ToString()
        {
            return $"Score: {Score}, Number of cards: {NumberOfCards}, Last card: {LastCard}";
        }
    }
}
