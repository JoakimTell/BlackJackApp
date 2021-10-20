using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    public class Card
    {

        #region Properties
        public Suit Suit { get; set; }

        public Value Value { get; set; }
        #endregion

        #region Constructors
        public Card(Suit suit, Value value)
        {
            Suit = suit;
            Value = value;
        }
        #endregion

        public override string ToString()
        {
            return $"{Value} of {Suit}";
        }
    }
}
