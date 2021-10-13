using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    public class Card
    {
        private Suit suit;
        private Value value;

        #region Properties
        public Suit Suit
        {
            get { return suit; }
            set { this.suit = value; }
        }

        public Value Value
        {
            get { return value; }
            set { this.value = value; }
        }
        #endregion

        public Card(Suit suit, Value value)
        {
            this.suit = suit;
            this.value = value;
        }

        public override string ToString()
        {
            return $"Suit: {suit} \n Value: {value}";
        }
    }
}
