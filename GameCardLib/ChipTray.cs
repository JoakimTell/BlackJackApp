using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public int PlayerBetScore { get; set; }

        public virtual Player Player { get; set; }

        public ChipTray()
        {
            ID = Guid.NewGuid().ToString("N");
            OneDollarChips = 25;
            FiveDollarChips = 10;
            TwentyDollarChips = 3;
        }

        public int TotalValue()
        {
            int value = 0;

            value += OneDollarChips * 1;
            value += FiveDollarChips * 5;
            value += TwentyDollarChips * 20;

            return value;
        }
    }
}
