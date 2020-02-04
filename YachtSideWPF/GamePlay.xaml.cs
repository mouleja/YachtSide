using System;
using System.Collections.Generic;
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
    /// Interaction logic for GamePlay.xaml
    /// </summary>
    public partial class GamePlay : UserControl, INotifyPropertyChanged
    {
        private int[] currentDice = { 0, 0, 0, 0, 0 };
        int roll = 1, turn = 1, playerIndex = 0, numPlayers = 1;
        string _currentPlayer = "";
        List<string> _players;
        Button[] p1UpperButtons, p1LowerButtons, p2UpperButtons, p2LowerButtons, p3UpperButtons, p3LowerButtons, p4UpperButtons, p4LowerButtons;
        Button[][] upperButtons, lowerButtons;
        TextBox[] p1scores, p2scores, p3scores, p4scores, die;
        TextBox[][] scores;
        CheckBox[] boxes;
        SolidColorBrush green;

        enum lb {Toak, Foak, Fh, Ss, Ls, Chance, Yat }  // Index of lower buttons in array
        enum sc {   // Index of scores in array
            Ones, Twos, Threes, Fours, Fives, Sixes, UpperTotal, UpperBonus,
            Toak, Foak, Fh, Ss, Ls, Chance, Yat, Yb, LowerTotal, Total
        }

        public GamePlay(List<string> players)
        {
            InitializeComponent();
            this.DataContext = this;

            CurrentPlayer = players[0];
            P1Name.Text = players[0];
            if (!String.IsNullOrEmpty(players[1]))
            {
                P2Grid.Visibility = Visibility.Visible;
                P2Name.Text = players[1];
                ++numPlayers;
            }
            if (!String.IsNullOrEmpty(players[2]))
            {
                P3Grid.Visibility = Visibility.Visible;
                P3Name.Text = players[2];
                ++numPlayers;
            }
            if (!String.IsNullOrEmpty(players[3]))
            {
                P4Grid.Visibility = Visibility.Visible;
                P4Name.Text = players[3];
                ++numPlayers;
            }
            _players = players;

            p1UpperButtons = new Button[] { P1OnesButton, P1TwosButton, P1ThreesButton, P1FoursButton, P1FivesButton, P1SixButton };
            p1LowerButtons = new Button[] { P1ToakButton, P1FoakButton, P1FhButton, P1SsButton, P1LsButton, P1ChanceButton, P1YatButton };
            p2UpperButtons = new Button[] { P2OnesButton, P2TwosButton, P2ThreesButton, P2FoursButton, P2FivesButton, P2SixButton };
            p2LowerButtons = new Button[] { P2ToakButton, P2FoakButton, P2FhButton, P2SsButton, P2LsButton, P2ChanceButton, P2YatButton };
            p3UpperButtons = new Button[] { P3OnesButton, P3TwosButton, P3ThreesButton, P3FoursButton, P3FivesButton, P3SixButton };
            p3LowerButtons = new Button[] { P3ToakButton, P3FoakButton, P3FhButton, P3SsButton, P3LsButton, P3ChanceButton, P3YatButton };
            p4UpperButtons = new Button[] { P4OnesButton, P4TwosButton, P4ThreesButton, P4FoursButton, P4FivesButton, P4SixButton };
            p4LowerButtons = new Button[] { P4ToakButton, P4FoakButton, P4FhButton, P4SsButton, P4LsButton, P4ChanceButton, P4YatButton };
            upperButtons = new Button[][] { p1UpperButtons, p2UpperButtons, p3UpperButtons, p4UpperButtons };
            lowerButtons = new Button[][] { p1LowerButtons, p2LowerButtons, p3LowerButtons, p4LowerButtons };
            
            p1scores = new TextBox[] { P1Ones, P1Twos, P1Threes, P1Fours, P1Fives, P1Sixes, p1upperTotal, p1upperBonus,
                p1toak, p1foak, p1fh, p1ss, p1ls, p1chance, p1yat, p1yb, p1lowerTotal, p1total };
            p2scores = new TextBox[] { P2Ones, P2Twos, P2Threes, P2Fours, P2Fives, P2Sixes, p2upperTotal, p2upperBonus,
                p2toak, p2foak, p2fh, p2ss, p2ls, p2chance, p2yat, p2yb, p2lowerTotal, p2total };
            p3scores = new TextBox[] { P3Ones, P3Twos, P3Threes, P3Fours, P3Fives, P3Sixes, p3upperTotal, p3upperBonus,
                p3toak, p3foak, p3fh, p3ss, p3ls, p3chance, p3yat, p3yb, p3lowerTotal, p3total };
            p4scores = new TextBox[] { P4Ones, P4Twos, P4Threes, P4Fours, P4Fives, P4Sixes, p4upperTotal, p4upperBonus,
                p4toak, p4foak, p4fh, p4ss, p4ls, p4chance, p4yat, p4yb, p4lowerTotal, p4total };
            scores = new TextBox[][] { p1scores, p2scores, p3scores, p4scores };

            boxes = new CheckBox[] { keep1, keep2, keep3, keep4, keep5 };
            die = new TextBox[] { die1, die2, die3, die4, die5 };
            green = new SolidColorBrush() { Color = Colors.LightGreen };

            GameOverTextBlock.Visibility = Visibility.Collapsed;
            PlayAgainButton.Visibility = Visibility.Collapsed;
            WinnerPanel.Visibility = Visibility.Collapsed;
            RollButton.Focus();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string CurrentPlayer {
            get
            {
                return _currentPlayer;
            }
            set
            {
                if (value != _currentPlayer)
                {
                    _currentPlayer = value;
                    NotifyPropertyChanged("CurrentPlayer");
                }
            }
        }

        private void RollButton_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();

            for (int i = 0; i < 5; ++i)
            {
                if (boxes[i].IsChecked == false)
                {
                    currentDice[i] = r.Next(1, 7);
                    die[i].Text = currentDice[i].ToString();
                }
            }

            ++roll;
            RollButton.Content = "Roll #" + roll.ToString();

            foreach (CheckBox b in boxes)
            {
                b.IsEnabled = true;
            }

            if (roll > 3)
            {
                RollButton.Content = "Final";
                RollButton.IsEnabled = false;

                foreach (CheckBox i in boxes)
                {
                    i.IsChecked = true;
                    i.IsEnabled = false;
                }
            }
            UpdateScores();
        }

        // Reset game to initial state
        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            turn = 1;
            ResetDie();
            for (int p = 0; p < 4; ++p)
            {
                foreach (TextBox s in scores[p])
                {
                    s.Text = "0";
                    s.Background = default(SolidColorBrush);
                }
                foreach (Button b in upperButtons[p].Concat(lowerButtons[p]))
                {
                    b.IsEnabled = true;
                }
            }

            TurnCounter.Text = "Turn # 1";

            GameOverTextBlock.Visibility = Visibility.Collapsed;
            PlayAgainButton.Visibility = Visibility.Collapsed;
            WinnerPanel.Visibility = Visibility.Collapsed;
        }

        private void UpdateScores() // After each roll, updates the scores in each column not already used
        {
            // update upper section scores
            for (int i = 0; i < 6; ++i)
            {
                if (upperButtons[playerIndex][i].IsEnabled)     // Calculate new scores for non-locked-in rows
                {
                    int score = 0;
                    for (int j = 0; j < 5; ++j)
                    {
                        if (currentDice[j] == i + 1)
                        {
                            score += i + 1;
                        }
                    }
                    scores[playerIndex][i].Text = score.ToString();
                }
            }

            // Calculate the number of each number rolled
            int[] numberOfEach = { 0, 0, 0, 0, 0, 0 };
            int totalDieScore = 0;
            for (int i = 0; i < 5; ++i)
            {
                numberOfEach[currentDice[i] - 1]++;
                totalDieScore += currentDice[i];
            }

            // Update lower section scores
            if (lowerButtons[playerIndex][(int)lb.Toak].IsEnabled && numberOfEach.Max() > 2)
            {
                scores[playerIndex][(int)sc.Toak].Text = totalDieScore.ToString();
            }

            if (lowerButtons[playerIndex][(int)lb.Foak].IsEnabled && numberOfEach.Max() > 3)
            {
                scores[playerIndex][(int)sc.Foak].Text = totalDieScore.ToString();
            }

            if (lowerButtons[playerIndex][(int)lb.Fh].IsEnabled && numberOfEach.Max() == 3 && numberOfEach.Contains(2))
            {
                scores[playerIndex][(int)sc.Fh].Text = "25";
            }

            if (lowerButtons[playerIndex][(int)lb.Ss].IsEnabled &&
                (
                    (numberOfEach[0] > 0 && numberOfEach[1] > 0 && numberOfEach[2] > 0 && numberOfEach[3] > 0) ||
                    (numberOfEach[1] > 0 && numberOfEach[2] > 0 && numberOfEach[3] > 0 && numberOfEach[4] > 0) ||
                    (numberOfEach[2] > 0 && numberOfEach[3] > 0 && numberOfEach[4] > 0 && numberOfEach[5] > 0))
                )
            {
                scores[playerIndex][(int)sc.Ss].Text = "30";
            }

            if (lowerButtons[playerIndex][(int)lb.Ls].IsEnabled &&
                (
                    (numberOfEach[0] > 0 && numberOfEach[1] > 0 && numberOfEach[2] > 0 && numberOfEach[3] > 0 && numberOfEach[4] > 0) ||
                    (numberOfEach[1] > 0 && numberOfEach[2] > 0 && numberOfEach[3] > 0 && numberOfEach[4] > 0 && numberOfEach[5] > 0))
                )
            {
                scores[playerIndex][(int)sc.Ls].Text = "40";
            }

            if (lowerButtons[playerIndex][(int)lb.Yat].IsEnabled && numberOfEach.Max() > 4)
            {
                scores[playerIndex][(int)sc.Yat].Text = "50";
            }

            if (lowerButtons[playerIndex][(int)lb.Chance].IsEnabled)
            {
                scores[playerIndex][(int)sc.Chance].Text = totalDieScore.ToString();
            }

            // Joker Rules (for additional YachtSides) - Using "Free Choice" Joker Rules from wikipedia
            if (!lowerButtons[playerIndex][(int)lb.Yat].IsEnabled && numberOfEach.Max() > 4)
            {
                if (scores[playerIndex][(int)sc.Yat].Text == "50") // YachtSide Bonus
                {
                    scores[playerIndex][(int)sc.Yb].Text = 
                        (int.Parse(scores[playerIndex][(int)sc.Yb].Text) + 100).ToString();
                }

                bool upperAvailable = false;
                for (int i = 0; i < 6; ++i)
                {
                    if (numberOfEach[i] == 5 && upperButtons[playerIndex][i].IsEnabled)
                    {
                        upperAvailable = true;
                    }
                }
                if (!upperAvailable)
                // joker rule: if another yachtzee roled and upper section for that number used, 
                // yachtzee counts as a joker
                {
                    scores[playerIndex][(int)sc.Fh].Text = "25";
                    scores[playerIndex][(int)sc.Ss].Text = "30";
                    scores[playerIndex][(int)sc.Ls].Text = "40";
                }
            }
        }

        private void SelectionButton_Click(object sender, RoutedEventArgs e)
        {
            Button selected = e.OriginalSource as Button;
            selected.IsEnabled = false; // Each button can only be selected once per game

            int total = 0;
            for (int i = 0; i < 6; ++i) // Add up upper scores and reset unselected to 0
            {
                if (!upperButtons[playerIndex][i].IsEnabled)
                {
                    total += int.Parse(scores[playerIndex][i].Text);
                    if (upperButtons[playerIndex][i] == selected)   // mark as already used
                    {
                        scores[playerIndex][i].Background = green;
                    }
                }
                else
                {
                    scores[playerIndex][i].Text = "0";
                }
            }

            if (total > 62)
            {
                scores[playerIndex][(int)sc.UpperBonus].Text = "35";
                total += 35;
            }
            scores[playerIndex][(int)sc.UpperTotal].Text = total.ToString();

            int lowerTotal = 0;
            for (int i = 8; i < 15; ++i)
            {
                if (!lowerButtons[playerIndex][i - 8].IsEnabled)
                {
                    lowerTotal += int.Parse(scores[playerIndex][i].Text);
                    if (lowerButtons[playerIndex][i - 8] == selected)
                    {
                        scores[playerIndex][i].Background = green;
                    }
                }
                else
                {
                    scores[playerIndex][i].Text = "0";
                }
            }
            lowerTotal += int.Parse(scores[playerIndex][(int)sc.Yb].Text);
            scores[playerIndex][(int)sc.LowerTotal].Text = lowerTotal.ToString();

            total += lowerTotal;
            scores[playerIndex][(int)sc.Total].Text = total.ToString();

            PassTheDice();

            ResetDie();
            RollButton.Focus();

            if (turn > 13)
            {
                RollButton.IsEnabled = false;
                GameOverTextBlock.Visibility = Visibility.Visible;
                PlayAgainButton.Visibility = Visibility.Visible;
                TurnCounter.Text = "";
                CalcScores();
            }
        }

        private void CalcScores()
        {
            List<int> gameScores = new List<int>();

            string path = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "YachtSide\\scores.dat");
            using (TextWriter tw = new StreamWriter(path, true))
            {
                for (int i = 0; i < numPlayers; ++i)
                {
                    string score = scores[i][(int)sc.Total].Text;

                    // Save to scores.dat
                    tw.WriteLine(DateTime.Today.ToShortDateString() + "," + score + "," + _players[i]);

                    gameScores.Add(int.Parse(score));
                }
            }

            if (numPlayers > 1)
            {
                int winnerIndex = gameScores.IndexOf(gameScores.Max());
                WinnerName.Text = _players[winnerIndex];
                WinnerPanel.Visibility = Visibility.Visible;
            }
        }

        private void PassTheDice()
        {
            playerIndex = (playerIndex + 1) % numPlayers;   // Rolling index
            if (playerIndex == 0)
            {
                turn++;
                TurnCounter.Text = "Turn # " + turn.ToString();
                P1Grid.IsEnabled = true;
                P2Grid.IsEnabled = false;
                P3Grid.IsEnabled = false;
                P4Grid.IsEnabled = false;
            }
            if (playerIndex == 1)
            { 
                P1Grid.IsEnabled = false;
                P2Grid.IsEnabled = true;
            }
            if (playerIndex == 2)
            {
                P2Grid.IsEnabled = false;
                P3Grid.IsEnabled = true;
            }
            if (playerIndex == 3)
            {
                P3Grid.IsEnabled = false;
                P4Grid.IsEnabled = true;
            }
            CurrentPlayer = _players[playerIndex];
        }

        private void ResetDie()
        {
            roll = 1;
            foreach (TextBox d in die)
            {
                d.Text = "X";
            }
            foreach (CheckBox b in boxes)
            {
                b.IsChecked = false;
                b.IsEnabled = false;
            }
            RollButton.Content = "Roll # 1";
            RollButton.IsEnabled = true;
        }
    }
}
