using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    class Player : Hand
    {
        public Hand Hand
        {
            get { return Hand; }
            set { Hand = value; }
        }

        public bool isFinishied
        {
            get { return isFinishied; }
            set { isFinishied = value; }
        }

        public string Name
        {
            get { return Name; }
            set { Name = value; }
        }

        public bool Winner
        {
            get { return Winner; }
            set { Winner = value; }
        }

        public void Player(string id, string name, Hand hand)
        {

        }

        public string ToString()
        {

        }


    }
}

