using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace YachtSide
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int[] currentDice = { 0, 0, 0, 0, 0 };
        int roll = 1, turn = 1;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void RollButton_Click(object sender, RoutedEventArgs e)
        {
            CheckBox[] boxes = { keep1, keep2, keep3, keep4, keep5 };
            TextBox[] die = { die1, die2, die3, die4, die5 };
            Random r = new Random();

            for (int i = 0; i < 5; ++i)
            {
                if (boxes[i].IsChecked == false)
                {
                    currentDice[i] = r.Next(1, 6);
                    die[i].Text = currentDice[i].ToString();
                }
            }

            ++roll;
            RollButton.Content = "Roll #" + roll.ToString();

            if (roll > 3)
            {
                RollButton.Content = "Final";
                RollButton.IsEnabled = false;

                foreach (CheckBox i in boxes)
                {
                    i.IsChecked = true;
                }
            }
            updateScores();
        }

        private void updateScores()
        {
            Button[] p1buttons = { OnesButton, TwosButton, ThreesButton, FoursButton, FivesButton, SixButton, ToakButton, FoakButton, FhButton, SsButton, LsButton, ChanceButton, YatButton };
            TextBox[] scores = { P1Ones, P1Twos, P1Threes, P1Fours, P1Fives, P1Sixes, p1upperTotal, p1upperBonus, p1toak, p1foak, p1fh, p1ss, p1ls, p1chance, p1yat };

            int upperTotal = 0, total = 0;
            for (int i = 0; i < 6; ++i)
            {
                if (p1buttons[i].IsEnabled)     // Calculate new scores for non-locked-in rows
                {
                    int score = 0;
                    for (int j = 0; j < 5; ++j)
                    {
                        if (currentDice[j] == i +1)
                        {
                            score += i + 1; 
                        }
                    }
                    scores[i].Text = score.ToString();
                    upperTotal += score;
                }
                else
                {
                    upperTotal += int.Parse(scores[i].Text);
                }
            }
            p1upperTotal.Text = upperTotal.ToString();

            total = upperTotal;
            if (upperTotal > 62)
            {
                p1upperBonus.Text = "35";
                total += 35;
            }

            // Calculate the number of each number rolled
            int[] numberOfEach = { 0, 0, 0, 0, 0, 0 };
            int totalDieScore = 0;
            for (int i = 0; i < 5; ++i)
            {
                numberOfEach[currentDice[i] - 1]++;
                totalDieScore += currentDice[i];
            }

            if (ToakButton.IsEnabled && numberOfEach.Max() > 2)
            {
                p1toak.Text = totalDieScore.ToString();
                total += totalDieScore;
            }
            else
            {
                total += int.Parse(p1toak.Text);
            }

            if (FoakButton.IsEnabled && numberOfEach.Max() > 3)
            {
                p1foak.Text = totalDieScore.ToString();
                total += totalDieScore;
            }
            else
            {
                total += int.Parse(p1foak.Text);
            }

            if (FhButton.IsEnabled && numberOfEach.Max() == 3 && numberOfEach.Contains(2))
            {
                p1fh.Text = "25";
                total += 25;
            }
            else
            {
                total += int.Parse(p1fh.Text);
            }


            p1total.Text = total.ToString();
        }

        private void SelectionButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
