using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilitiesLib;


namespace BlackJackApp
{
    public class Deck
    {
        private ListManager<Card> cards;
        private readonly Random randomArranger;

        #region Properties
        public bool GameIsDone { get; set; }
        #endregion

        // Add a card to the deck.
        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        // Construct with a list of cards as parameter.
        public Deck(List<Card> cardList)
        {
            InitializeDeck(cardList);
        }

        // Fill deck of 52 cards.
        public void InitializeDeck(List<Card> cardList)
        {
            DiscardCards();
            cards = new ListManager<Card>(cardList);
            for (int s = 0; s < 4; s++) // Enum values of the four suites. 
            {
                for (int v = 1; v <= 13; v++) // Enum values from Ace to King.
                {
                    cards.Add(new Card((Suit)s, (Value)v));
                }
            }
            Shuffle();
            ToString();
            Debug.WriteLine($"Sum of values of the cards in the deck: {SumOfCards()}");
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
        public void OnShuffle(Object source, EventArgs eventArgs) // TODO: Event?
        {
            Shuffle();
        }

        private void Shuffle()
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

        // Remove one card from the deck, at a specified position. 
        public void RemoveCard(int pos)
        {
            cards.DeleteAt(pos);
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
            int columns = 5;// Debugging.
            foreach (Card card in cards.List)
            {
                cardsInDeck.AppendLine(card.ToString());

                // Start Debugging.
                if (columns == 0)
                {
                    Debug.Write($"{card,20}\n");
                    columns = 5;
                }
                else
                {
                    Debug.Write($"{card,20}");
                    columns--;
                }
                // End Debugging.
            }
            return cardsInDeck.ToString();
        }
    }
}
