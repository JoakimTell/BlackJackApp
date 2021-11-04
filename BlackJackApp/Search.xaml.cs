using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            lstViewSearchResults.Items.Clear();
            string searchString = txtSearch.Text;
            PlayerContext context = new PlayerContext();
            List<Player> foundPlayers = PlayerContext.FindPlayer(searchString, context);

            if (lstViewSearchResults.Items.Count == 0)
            {
                foreach (Player player in foundPlayers)
                {
                    if (!(player.Name == "Dealer"))
                    {
                        Debug.WriteLine("Name: " + player.Name + " --- ID: " + player.PlayerID);
                        lstViewSearchResults.Items.Add(player);
                    }
                }
            }
            else
            {
                lstViewSearchResults.Items.Refresh();
            }

            context.Dispose();
        }
    }
}
