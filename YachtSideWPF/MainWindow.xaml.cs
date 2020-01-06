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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YachtSideWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int[] currentDice = { 0, 0, 0, 0, 0 };
        int roll = 1, turn = 1;
        Button[] p1UpperButtons, p1LowerButtons;
        TextBox[] scores, die;
        CheckBox[] boxes;
        SolidColorBrush green;

        public MainWindow()
        {
            InitializeComponent();

            p1UpperButtons = new Button[] { OnesButton, TwosButton, ThreesButton, FoursButton, FivesButton, SixButton };
            p1LowerButtons = new Button[] { ToakButton, FoakButton, FhButton, SsButton, LsButton, ChanceButton, YatButton };
            scores = new TextBox[] { P1Ones, P1Twos, P1Threes, P1Fours, P1Fives, P1Sixes, p1upperTotal, p1upperBonus,
                p1toak, p1foak, p1fh, p1ss, p1ls, p1chance, p1yat, p1yb, p1lowerTotal, p1total };
            boxes = new CheckBox[] { keep1, keep2, keep3, keep4, keep5 };
            die = new TextBox[] { die1, die2, die3, die4, die5 };
            green = new SolidColorBrush() { Color = Colors.LightGreen };

            RollButton.Focus();
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
            foreach (TextBox s in scores)
            {
                s.Text = "0";
                s.Background = default(SolidColorBrush);
            }
            foreach (Button b in p1UpperButtons.Concat(p1LowerButtons))
            {
                b.IsEnabled = true;
            }
            GameOverTextBlock.Visibility = Visibility.Collapsed;
            PlayAgainButton.Visibility = Visibility.Collapsed;
        }

        private void UpdateScores() // After each roll, updates the scores in each column not already used
        {
            // update upper section scores
            for (int i = 0; i < 6; ++i)
            {
                if (p1UpperButtons[i].IsEnabled)     // Calculate new scores for non-locked-in rows
                {
                    int score = 0;
                    for (int j = 0; j < 5; ++j)
                    {
                        if (currentDice[j] == i + 1)
                        {
                            score += i + 1;
                        }
                    }
                    scores[i].Text = score.ToString();
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
            if (ToakButton.IsEnabled && numberOfEach.Max() > 2)
            {
                p1toak.Text = totalDieScore.ToString();
            }

            if (FoakButton.IsEnabled && numberOfEach.Max() > 3)
            {
                p1foak.Text = totalDieScore.ToString();
            }

            if (FhButton.IsEnabled && numberOfEach.Max() == 3 && numberOfEach.Contains(2))
            {
                p1fh.Text = "25";
            }

            if (SsButton.IsEnabled &&
                (
                    (numberOfEach[0] > 0 && numberOfEach[1] > 0 && numberOfEach[2] > 0 && numberOfEach[3] > 0) ||
                    (numberOfEach[1] > 0 && numberOfEach[2] > 0 && numberOfEach[3] > 0 && numberOfEach[4] > 0) ||
                    (numberOfEach[2] > 0 && numberOfEach[3] > 0 && numberOfEach[4] > 0 && numberOfEach[5] > 0))
                )
            {
                p1ss.Text = "30";
            }

            if (LsButton.IsEnabled &&
                (
                    (numberOfEach[0] > 0 && numberOfEach[1] > 0 && numberOfEach[2] > 0 && numberOfEach[3] > 0 && numberOfEach[4] > 0) ||
                    (numberOfEach[1] > 0 && numberOfEach[2] > 0 && numberOfEach[3] > 0 && numberOfEach[4] > 0 && numberOfEach[5] > 0))
                )
            {
                p1ls.Text = "40";
            }

            if (YatButton.IsEnabled && numberOfEach.Max() > 4)
            {
                p1yat.Text = "50";
            }

            if (ChanceButton.IsEnabled)
            {
                p1chance.Text = totalDieScore.ToString();
            }

            // Joker Rules (for additional YachtSides) - Using "Free Choice" Joker Rules from wikipedia
            if (!YatButton.IsEnabled && numberOfEach.Max() > 4)
            {
                if (p1yat.Text == "50") // YachtSide Bonus
                {
                    p1yb.Text = (int.Parse(p1yb.Text) + 100).ToString();
                }

                bool upperAvailable = false;
                for (int i = 0; i < 6; ++i)
                {
                    if (numberOfEach[i] == 5 && p1UpperButtons[i].IsEnabled)
                    {
                        upperAvailable = true;
                    }
                }
                if (!upperAvailable)
                // joker rule: if another yachtzee roled and upper section for that number used, 
                // yachtzee counts as a joker
                {
                    p1fh.Text = "25";
                    p1ss.Text = "30";
                    p1ls.Text = "40";
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
                if (!p1UpperButtons[i].IsEnabled)
                {
                    total += int.Parse(scores[i].Text);
                    if (p1UpperButtons[i] == selected)
                    {
                        scores[i].Background = green;
                    }
                }
                else
                {
                    scores[i].Text = "0";
                }
            }

            if (total > 62)
            {
                p1upperBonus.Text = "35";
                total += 35;
            }
            p1upperTotal.Text = total.ToString();

            int lowerTotal = 0;
            for (int i = 8; i < 15; ++i)
            {
                if (!p1LowerButtons[i - 8].IsEnabled)
                {
                    lowerTotal += int.Parse(scores[i].Text);
                    if (p1LowerButtons[i - 8] == selected)
                    {
                        scores[i].Background = green;
                    }
                }
                else
                {
                    scores[i].Text = "0";
                }
            }
            lowerTotal += int.Parse(p1yb.Text);
            p1lowerTotal.Text = lowerTotal.ToString();

            total += lowerTotal;
            p1total.Text = total.ToString();

            turn++;
            TurnCounter.Text = "Turn # " + turn.ToString();

            ResetDie();
            RollButton.Focus();

            if (turn > 13)
            {
                RollButton.IsEnabled = false;
                GameOverTextBlock.Visibility = Visibility.Visible;
                PlayAgainButton.Visibility = Visibility.Visible;
                TurnCounter.Text = "Turn # 1";
            }
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