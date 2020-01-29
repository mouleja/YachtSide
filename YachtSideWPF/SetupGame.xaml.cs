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

        string _currentPlayer;

        public SetupGame()
        {
            InitializeComponent();
            getPlayers();
            PlayerComboBox.ItemsSource = _players;
        }

        private void getPlayers()
        {
            if (File.Exists(path))
            {
                _players = new ObservableCollection<string>(File.ReadAllLines(path));
            }
            else
            {
                _players = new ObservableCollection<string>();
                _players.Add("");
            }
        }

        public ObservableCollection<string> PlayerList
        {
            get { return _players; }
        }


        private void PlayerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.Source as ComboBox;
            _currentPlayer = selected.SelectedItem.ToString();
        }

        private void AddNewPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPlayer = NewPlayerTextBox.Text;
            _players.Add(_currentPlayer);
            saveStringList(_players);
            NewPlayerTextBox.Text = "";
            File.Create(dirPath + "\\Scores\\" + _currentPlayer + ".dat").Dispose();
        }

        private void saveStringList(ObservableCollection<string> list)
        {
            if (!(File.Exists(path)))
            {
                Directory.CreateDirectory(dirPath);
                Directory.CreateDirectory(dirPath + "\\Scores");
                File.Create(dirPath + "\\Scores\\alltime.dat").Dispose();
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
    }
}
