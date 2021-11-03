using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BlackJackApp
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        public Search()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string searchString = txtSearch.Text;
            List<Player> foundPlayers = PlayerContext.FindPlayer(searchString);

            if (lstViewSearchResults.Items.Count == 0)
            {
                foreach (Player player in foundPlayers)
                {
                    if (!(player.Name == "Dealer"))
                    {
                        var row = new { player.Name, player.Wins, player.Losses };
                        lstViewSearchResults.Items.Add(player);
                    }
                }
            }
            else
            {
                lstViewSearchResults.Items.Refresh();
            }
        }
    }
}
