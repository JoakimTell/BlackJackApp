using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
    public class ChipTray
    {
        public string ID { get; set; }
        public int OneDollarChips { get; set; }
        public int FiveDollarChips { get; set; }
        public int TwentyDollarChips { get; set; }

        public virtual Player Player { get; set; }

        public ChipTray()
        {
            ID = Guid.NewGuid().ToString("N");
            OneDollarChips = 0;
            FiveDollarChips = 0;
            TwentyDollarChips = 0;
        }
    }
}
