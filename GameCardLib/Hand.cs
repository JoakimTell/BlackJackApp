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
            Cards = new List<Card>();
        }
        #endregion

        #region Properties
        public Card LastCard { get; set; }

        public List<Card> Cards { get; set; }

        public int NumberOfCards { get; set; }

        public int Score { get; set; }
        #endregion

        public void AddCard(Card card)
        {
            LastCard = card;
            Cards.Add(card);
            Score += (int) card.Value;
            NumberOfCards++;
        }

        public void Clear()
        {
            LastCard = null;
            Cards.Clear();
            NumberOfCards = 0;
            Score = 0;
        }

        public override string ToString()
        {
            return $"Score: {Score}, Number of cards: {NumberOfCards}, Last card: {LastCard}";
        }
    }
}
