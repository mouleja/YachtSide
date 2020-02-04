using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
    /// Interaction logic for Scores.xaml
    /// </summary>
    public partial class ScoresUserControl : UserControl, INotifyPropertyChanged
    {
        UserControl _priorUC;
        MainWindow _parent;
        List<ScoreEntry> _scores = new List<ScoreEntry>();
        ObservableCollection<ScoreEntry> _topScores;
        ObservableCollection<string> _names;
        ObservableCollection<ScoreEntry> _selectedScores;

        public event PropertyChangedEventHandler PropertyChanged;

        public ScoresUserControl(MainWindow parent, UserControl priorUC)
        {
            InitializeComponent();
            this.DataContext = this;

            _priorUC = priorUC;
            _parent = parent;

            ReadScoresFromFile();
            //SelectedScoresListView.ItemsSource = SelectedScores;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<ScoreEntry> AllScores {
            get
            {
                return _topScores;
            }

            set
            {
                _topScores = value;
                NotifyPropertyChanged("AllScores");
            }
        }

        public ObservableCollection<string> Names
        {
            get
            {
                return _names;
            }

            set
            {
                _names = value;
                NotifyPropertyChanged("Names");
            }
        }

        public ObservableCollection<ScoreEntry> SelectedScores
        {
            get
            {
                return _selectedScores;
            }

            set
            {
                _selectedScores = value;
                NotifyPropertyChanged("SelectedScores");
            }
        }

        private void ReadScoresFromFile()
        {
            string path = System.IO.Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "YachtSide\\scores.dat");

            List<string> allScores;

            if (File.Exists(path))
            {
                allScores = new List<string>(File.ReadAllLines(path));
            }
            else { throw new FileNotFoundException(); }

            foreach (string score in allScores)
            {
                string[] temp = score.Split(',');
                _scores.Add(new ScoreEntry(temp[0], int.Parse(temp[1]), temp[2]));
            }

            AllScores = new ObservableCollection<ScoreEntry>(_scores.OrderByDescending(a => a.Score).Take(10));
            Names = new ObservableCollection<string>(_scores.Select(x => x.Name).Distinct());
        }

        private void CloseScoresButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.CurrentControl = _priorUC;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = e.Source as ComboBox;
            string selected = box.SelectedItem.ToString();
            SelectedScores = new ObservableCollection<ScoreEntry>(
                _scores.Where(x => x.Name == selected).OrderByDescending(x => x.Score).Take(10));
        }
    }
}
