using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilitiesLib;


namespace BlackJackApp
{
    class Deck
    {
        private ListManager<Card> cards;
        private Random randomArranger;

        private bool gameIsDone;

        public bool GameIsDone
        {
            get { return gameIsDone; }
            set { this.gameIsDone = value; }
        }

        // 
        public void AddCard(Card card)
        {
            // TODO: Typ hit
        }

        // Construct with a list of cards as parameter.
        public Deck (List<Card> cardList)
        {
            InitializeDeck(cardList);
        }

        // 
        public void InitializeDeck(List<Card> cardList)
        {
            cards = new ListManager<Card>(cardList);
            // TODO: Do something more with the list???
        }

        // 
        public void DiscardCards()
        {
            // TODO: Start new game?
        }

        // Return a specified card.
        public Card GetAt(int position)
        {
            return cards.GetAt(position);
        }

        // Return the two first cards from the deck.
        public List<Card> getTwoCards()
        {
            List<Card> lst = new();

            for(int i = 0; i < 2; i++)
            {
                lock (cards)
                {
                    lst.Add(cards.GetAt(0));
                    cards.DeleteAt(0);
                }
            }
            
            return lst;
        }

        // Return how many cards are left in the deck.
        public int NumberOfCards()
        {
            // TODO: What number? decks number of cards?
            return cards.Count;
        }

        // Fisher–Yates shuffle
        public void OnShuffle(Object source, EventArgs eventArgs)
        {
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = randomArranger.Next(n + 1);
                Card value = cards.GetAt(k);
                cards.ChangeAt(cards.GetAt(n), k);
                cards.ChangeAt(value, n);
            }
            // TODO: This is an event. Use with ButtonShuffle somehow?
        }

        // 
        public void RemoveCard(int pos)
        {
            // TODO: Why use this method?
            cards.DeleteAt(pos);
        }

        public int SumOfCards()
        {
            // TODO: Points of a hand? 
        }

        public override string ToString()
        {
            return ;
        }
 
    }
}
