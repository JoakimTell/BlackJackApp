using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilitiesLib;


namespace BlackJackApp
{
    public delegate void OnDeckIsRunninOutEventHandler(object source, EventArgs eventArgs);

    public class Deck
    {
        private ListManager<Card> cards;
        private readonly Random randomArranger;
        private int nbrOfDecks;

        public event OnDeckIsRunninOutEventHandler DeckIsRunningOut;

        #region Properties
        public bool GameIsDone { get; set; }
        #endregion

        public Deck(List<Card> cardList)
        {
            randomArranger = new Random();
            cards = new ListManager<Card>(cardList);
        }

        // Fill deck of 52 cards.
        public void InitializeNewDeck(int nbrOfDecks)
        {
            this.nbrOfDecks = nbrOfDecks;
            DiscardCards();
            for (int d = 0; d < nbrOfDecks; d++)
            {
                for (int s = 0; s < 4; s++) // Enum values of the four suites. 
                {
                    for (int v = 1; v <= 13; v++) // Enum values from Ace to King.
                    {
                        cards.Add(new Card((Suit)s, (Value)v));
                    }
                }
            }
            //ToString();
        }

        // Remove all cards from the deck.
        public void DiscardCards()
        {
            cards.DeleteAll();
        }

        // Return a specified card.
        public Card GetAt(int position)
        {
            return cards.GetAt(position);
        }

        // Return the two first cards from the deck.
        public List<Card> GetTwoCards()
        {
            List<Card> lst = new();

            for (int i = 0; i < 2; i++)
            {
                lock (cards)
                {
                    lst.Add(GetAt(0));
                    RemoveCard(0);
                }
            }

            return lst;
        }

        // Return how many cards are left in the deck.
        public int NumberOfCards()
        {
            return cards.Count;
        }

        // Fisher–Yates shuffle.
        public void Shuffle()
        {
            int n = NumberOfCards();
            while (n > 1)
            {
                n--;
                int k = randomArranger.Next(n + 1);
                Card value = cards.GetAt(k);
                cards.ChangeAt(cards.GetAt(n), k);
                cards.ChangeAt(value, n);
            }
        }

        // Add a card to the deck.
        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        // Remove one card from the deck, at a specified position. 
        public void RemoveCard(int pos)
        {
            cards.DeleteAt(pos);

            if (cards.Count == 46) // typ 25% kvar senare...
            {
                DeckIsRunningOut?.Invoke(this, EventArgs.Empty);
                //Debug.WriteLine("From Deck: Only " + cards.Count + " cards left in the deck!");
                //ToString();
                //Debug.WriteLine("");
                //Debug.WriteLine("New deck: ");
                InitializeNewDeck(nbrOfDecks);
                Shuffle();
            }
        }

        // Value of all cards in the deck combined.
        public int SumOfCards()
        {
            int sumValue = 0;
            foreach (Card card in cards.List)
            {
                sumValue += (int)card.Value;
            }
            return sumValue;
        }

        // Print all cards remaining in the deck
        public override string ToString()
        {
            StringBuilder cardsInDeck = new();
            foreach (Card card in cards.List)
            {
                cardsInDeck.AppendLine(card.ToString());
            }
            return cardsInDeck.ToString();
        }
    }
}
