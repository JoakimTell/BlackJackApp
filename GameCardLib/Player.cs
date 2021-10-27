using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    public class Player
    {

        #region Properties
        public Hand Hand { get; set; }

        public bool IsFinnishied { get; set; }

        public string Name { get; set; }

        public string PlayerID { get; set; }

        public bool Winner
        {
            get;
            set;
        }

        public int Wins { get; set; }

        public int Losses { get; set; }
        #endregion

        #region Constructors
        public Player(string id, string name, Hand hand, int wins, int loses)
        {
            PlayerID = id;
            Name = name;
            Hand = hand;
            Wins = wins;
            Losses = loses;
        }

        #endregion

        public override string ToString()
        {
            return $"{Name} {(Winner ? "is a winner!" : "lost...")}";
        }
    }
}

