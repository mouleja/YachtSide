using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YachtSideWPF
{
    /// <summary>
    /// Interaction logic for SetupGame.xaml
    /// </summary>
    public partial class SetupGame : UserControl
    {
        string dirPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
            "YachtSide");
        string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "YachtSide", "players.dat");

        ObservableCollection<string> _players;
        int _numberOfPlayers = 1;
        List<string> _currentPlayers = new List<string> {"", "", "", ""};
        MainWindow _parent;

        public SetupGame(MainWindow parent)
        {
            InitializeComponent();
            _parent = parent;
            getPlayers();
            Player1CB.ItemsSource = _players;
            Player2CB.ItemsSource = _players;
            Player3CB.ItemsSource = _players;
            Player4CB.ItemsSource = _players;
        }

        public List<string> PlayerList { 
            get 
            { 
                return _currentPlayers; 
            } 
        }

        public int NumberOfPlayers {
            get 
            {
                return _numberOfPlayers;
            }
        }

        private void getPlayers()
        {
            if (File.Exists(path))
            {
                _players = new ObservableCollection<string>(File.ReadAllLines(path));
            }
            else
            {
                _players = new ObservableCollection<string>() { "", "yachtBot" };
            }
        }

        private void PlayerCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.Source as ComboBox;
            int playerNumber = int.Parse(selected.Name[6].ToString());  // index 6 is the player number in "PlayerXCB"
            _currentPlayers[playerNumber - 1] = selected.SelectedItem.ToString();
            CheckStartCondition();
        }

        private void AddNewPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            string newPlayer = NewPlayerTextBox.Text;

            if (_players.Contains(newPlayer))
            {
                AddPlayerError.Visibility = Visibility.Visible;
            }
            else
            {
                AddPlayerError.Visibility = Visibility.Hidden;
                _players.Add(newPlayer);
                saveStringList(_players);
                NewPlayerTextBox.Text = "";
                File.Create(dirPath + "\\Scores\\" + newPlayer + ".dat").Dispose();
            }
        }

        private void saveStringList(ObservableCollection<string> list)
        {
            if (!(File.Exists(path)))
            {
                Directory.CreateDirectory(dirPath);
                Directory.CreateDirectory(dirPath + "\\Scores");
                File.Create(dirPath + "\\Scores\\alltime.dat").Dispose();
                File.Create(dirPath + "\\Scores\\yachtBot.dat").Dispose();
                File.Create(path).Dispose();    // Dispose gets rid of error on next line
            }

            using (TextWriter tw = new StreamWriter(path))
            {

                foreach (string s in list)
                {
                    tw.WriteLine(s);
                }
            }
        }

        private void NumberOfPlayersCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = e.Source as ComboBox;
            int newNumber = cb.SelectedIndex + 1;

            if (newNumber > _numberOfPlayers)
            {
                if (newNumber > 1) SelectPlayer2.Visibility = Visibility.Visible;
                if (newNumber > 2) SelectPlayer3.Visibility = Visibility.Visible;
                if (newNumber > 3) SelectPlayer4.Visibility = Visibility.Visible;
            }
            else
            {
                if (newNumber < 4 && !(SelectPlayer4 is null))
                {
                    SelectPlayer4.Visibility = Visibility.Collapsed;
                    _currentPlayers[3] = "";
                    Player4CB.SelectedIndex = 0;
                }
                if (newNumber < 3 && !(SelectPlayer3 is null))
                {
                    SelectPlayer3.Visibility = Visibility.Collapsed;
                    _currentPlayers[2] = "";
                    Player3CB.SelectedIndex = 0;
                }
                if (newNumber < 2 && !(SelectPlayer2 is null))
                {
                    SelectPlayer2.Visibility = Visibility.Collapsed;
                    _currentPlayers[1] = "";
                    Player2CB.SelectedIndex = 0;
                }
            }
            _numberOfPlayers = newNumber;
            CheckStartCondition();
        }

        private void CheckStartCondition()
        {
            if (StartGameButton is null) return;

            bool okay = true;
            for (int i = 0; i < _numberOfPlayers; ++i)
            {
                if (string.IsNullOrWhiteSpace(_currentPlayers[i]))
                {
                    okay = false;
                }
            }
            if (okay)
            {
                StartGameButton.IsEnabled = true;
            }
            else
            {
                StartGameButton.IsEnabled = false;
            }
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.CurrentControl = new GamePlay();
        }
    }
}
