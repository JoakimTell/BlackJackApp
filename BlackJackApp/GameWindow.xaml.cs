using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void Hit_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Stay_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Shuffle_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void New_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }

        private void StartNewGame()
        {
            //Reset the game 
            //Create nmbrOfPlayer
            //Create nmbrOfDealer
            //Player dealer = new Player();
            //dealer.addcard()
            string projectPathLocal = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            string path = System.IO.Path.Combine(projectPathLocal, @"Assets\Cards\", "c01.png");

            Uri uri = new Uri(path);
            Debug.WriteLine(uri);
            playerCard1.Source = new BitmapImage(uri);
            
        }
    }
}

