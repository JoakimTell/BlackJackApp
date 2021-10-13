using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    class Player
    {
        private Hand hand;
        private bool isFinished;
        private string name;
        private bool winner;
        private string playerID;

        #region Properties
        public Hand Hand
        {
            get { return hand; }
            set { hand = value; }
        }

        public bool isFinishied
        {
            get { return isFinishied; }
            set { isFinishied = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string PlayerID
        {
            get { return playerID; }
            set { playerID = value; }
        }

        public bool Winner
        {
            get { return winner; }
            set { winner = value; }
        }
        #endregion

        public Player(string id, string name, Hand hand)
        {
            this.playerID = id;
            this.name = name;
            this.hand = hand;
        }

        public override string ToString()
        {
            return $"Name: {name} {(winner ? "is a winner!" : "lost...")}";
        }


    }
}

