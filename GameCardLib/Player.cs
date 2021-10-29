using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BlackJackApp
{
    [Serializable]
    public class Player
    {

        #region Properties
        [XmlIgnore]
        public Hand Hand { get; set; }
        [XmlIgnore]
        public bool IsFinnishied { get; set; }
        [XmlIgnore]
        public string Name { get; set; }

        public string PlayerID { get; set; }
        [XmlIgnore]
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

        public Player()
        {

        }

        #endregion

        public override string ToString()
        {
            return $"{Name} {(Winner ? "is a winner!" : "lost...")}";
        }
    }
}

