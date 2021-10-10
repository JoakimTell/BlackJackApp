using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    class Card
    {
        public Suit Suit
        {
            get { return Suit; }
            set { Suit = value; }
        }

        public Value Value
        {
            get { return Value; }
            set { Value = value; }
        }

        public void Card(Suit suit, Value value)
        {

        }

        public string ToString()
        {

        }
    }
}
