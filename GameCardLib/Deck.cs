using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    class Deck : Card
    {
        private ListManager<Card> cards;
        private Random RandomArranger;

        public bool GameIsDone
        {
            get { return GameIsDone; }
            set { GameIsDone = value; }
        }

        public void AddCard(Card card)
        {

        }

        //DECKLISTmetoden i classdiagram fråga gitmaster
        public Deck (List<Card> cardList)
        {

        }
        
        public void DiscardCards()
        {

        }

        public Card GetAt(int position)
        {

        }

        public List<Card> getTwoCards()
        {

        }

        public void InitializeDeck (List<Card> cardList)
        {

        }

        public int NumberOfCards()
        {

        }

        public void OnShuffle(Object source, EventArgs eventArgs)
        {

        }

        public void RemoveCard(int pos)
        {

        }

        public int SumOfCards()
        {

        }

        public string ToString()
        {

        }
 
    }
}
