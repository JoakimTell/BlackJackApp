using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
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
        [NotMapped]
        public Hand Hand { get; set; }
        [XmlIgnore]
        public bool IsFinnishied { get; set; }

        public string Name { get; set; }
        [XmlIgnore]
        public string PlayerID { get; set; }
        [XmlIgnore]
        public bool Winner { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public virtual List<ChipTray> ChipTrays { get; set; }
        #endregion

        public void On_Deck_LowOnCards_Player(object sender, EventArgs e)
        {
            Debug.WriteLine("Player " + PlayerID + " method reached!");
        }

        #region Constructors
        public Player(int id, string name, Hand hand)
        {
            PlayerID = id.ToString();
            //PlayerID = Guid.NewGuid().ToString("N");
            Name = name;
            Hand = hand;
            Wins = 0;
            Losses = 0;
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

