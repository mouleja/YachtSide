using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
        TextBox[] p1scores, p2scores, p3scores, p4scores;
        TextBox[][] scores;
        CheckBox[] boxes;
        SolidColorBrush green;

        enum lb { Toak, Foak, Fh, Ss, Ls, Chance, Yat }  // Index of lower buttons in array
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
            Die1 = "X"; Die2 = "X"; Die3 = "X"; Die4 = "X"; Die5 = "X";

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
            green = new SolidColorBrush() { Color = Colors.LightGreen };

            GameOverTextBlock.Visibility = Visibility.Collapsed;
            PlayAgainButton.Visibility = Visibility.Collapsed;
            WinnerPanel.Visibility = Visibility.Collapsed;

            if (CurrentPlayer == "yachtBot")
            {
                RollButton.Content = "Start yachtBot!";
            }
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

        private string _die1, _die2, _die3, _die4, _die5;

        public string Die1
        {
            get { return _die1; }
            set { _die1 = value; NotifyPropertyChanged("Die1"); }
        }

        public string Die2
        {
            get { return _die2; }
            set { _die2 = value; NotifyPropertyChanged("Die2"); }
        }

        public string Die3
        {
            get { return _die3; }
            set { _die3 = value; NotifyPropertyChanged("Die3"); }
        }

        public string Die4
        {
            get { return _die4; }
            set { _die4 = value; NotifyPropertyChanged("Die4"); }
        }

        public string Die5
        {
            get { return _die5; }
            set { _die5 = value; NotifyPropertyChanged("Die5"); }
        }

        private void RollButton_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();

            for (int i = 0; i < 5; ++i)
            {
                if (boxes[i].IsChecked == false)
                {
                    currentDice[i] = r.Next(1, 7);
                }
            }
            Die1 = currentDice[0].ToString();
            Die2 = currentDice[1].ToString();
            Die3 = currentDice[2].ToString();
            Die4 = currentDice[3].ToString();
            Die5 = currentDice[4].ToString();

            if (CurrentPlayer == "yachtBot")
            {
                // http://www.jonathanantoine.com/2011/08/29/update-my-ui-now-how-to-wait-for-the-rendering-to-finish/
                Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
                Thread.Sleep(1000);
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

            if (CurrentPlayer == "yachtBot")
            {
                yachtBotsTurn().RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
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

            if (CurrentPlayer == "yachtBot")
            {
                RollButton.Content = "Start yachtBot!";
            }
            RollButton.Focus();

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

            RollButton.Focus();

            PassTheDice();

            if (turn > 13)
            {
                RollButton.IsEnabled = false;
                GameOverTextBlock.Visibility = Visibility.Visible;
                PlayAgainButton.Visibility = Visibility.Visible;
                TurnCounter.Text = "";
                CalcScores();
            }
            else if (CurrentPlayer == "yachtBot")
            {
                RollButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
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
            
            if (CurrentPlayer == "yachtBot" && turn < 2)
            {
                RollButton.Content = "Start yachtBot!";
            }
            ResetDie();
            RollButton.Focus();
        }

        private Button yachtBotsTurn()
        {
            // based on https://en.wikipedia.org/wiki/Yahtzee

            int[] diceCount = GetDiceCount(); ;
            (int mostDie, int highDie, int totalScore, bool fh, bool ss, bool ls) = getHand(diceCount);

            foreach (CheckBox c in boxes)   // Reset checkboxes for dice to roll
            {
                c.IsChecked = false;
            }

            if (roll == 2)
            {
                // Exceptions before second roll
                if (fh && highDie == 1) // full house with 3 1's
                {
                    return lowerButtons[playerIndex][(int)lb.Fh];
                }
                if (ss && diceCount[5] == 0 && diceCount[4] == 2)   // 12344
                {
                    keepThese(new int[] { 4 });
                }
                else if (ss && diceCount[2] == 0 && mostDie == 2)   // 3456 + pair
                {
                    keepThese(new int[] { highDie });
                }
                else if (mostDie == 2 && highDie == 1)  // don't keep pairs of 1s
                {
                    if (diceCount[3] == 1 && diceCount[4] == 1 && diceCount[5] == 1)
                    {
                        keepThese(new int[] { 3, 4, 5 });
                    }
                    else
                    {
                        foreach (int i in new int[] { 5, 4, 6 })
                        {
                            if (diceCount[i] == 1)
                            {
                                keepThese(new int[] { i });
                            }
                        }
                    }
                }
                else    // Default actions
                {
                    if (ls)     // Keep large straights
                    {
                        return lowerButtons[playerIndex][(int)lb.Ls];
                    }

                    else if (ss || lsPossible(diceCount))
                    {
                        if (diceCount[5] == 0)
                        {
                            keepThese(new int[] { 1, 2, 3, 4 });
                        }
                        else if (diceCount[2] == 0)
                        {
                            keepThese(new int[] { 3, 4, 5, 6 });
                        }
                        else
                        {
                            keepThese(new int[] { 2, 3, 4, 5 });
                        }

                        if (mostDie == 2)   // uncheck duplicates in small straight
                        {
                            uncheckOne(highDie);
                        }
                    }
                    else
                    {
                        keepThese(new int[] { highDie });   // Default keep larget number of die, highest if tie
                    }
                }
                return RollButton;
            }
            // Second roll
            if (roll == 3)
            {
                // Exceptions before third role
                if (fh && highDie < 4)  // Keep full houses w/ 3 3s or lower
                {
                    return lowerButtons[playerIndex][(int)lb.Fh];
                }
                if (mostDie == 2 && diceCount[1] == 2 && highDie < 4)   // 1122x or 1133x or 11xxx
                {
                    if (diceCount[3] == 1 && diceCount[4] == 1 && diceCount[5] == 1)
                    {
                        keepThese(new int[] { 3, 4, 5 });
                    }
                    else
                    {
                        keepThese(new int[] { 1, highDie });
                    }
                }
                else if (mostDie == 1 && diceCount[3] == 0) // 12456
                {
                    keepThese(new int[] { 4, 5, 6 });
                }
                else    // Default actions (same as above)
                {
                    if (ls)     // Keep large straights
                    {
                        return lowerButtons[playerIndex][(int)lb.Ls];
                    }

                    else if (ss || lsPossible(diceCount))
                    {
                        if (diceCount[5] == 0)
                        {
                            keepThese(new int[] { 1, 2, 3, 4 });
                        }
                        else if (diceCount[2] == 0)
                        {
                            keepThese(new int[] { 3, 4, 5, 6 });
                        }
                        else
                        {
                            keepThese(new int[] { 2, 3, 4, 5 });
                        }

                        if (mostDie == 2)   // uncheck duplicates in small straight
                        {
                            uncheckOne(highDie);
                        }
                    }
                    else
                    {
                        keepThese(new int[] { highDie });   // Default keep largest number of die, highest if tie
                    }
                }
                return RollButton;
            }
            if (roll == 4)  // Rolls done need to select category
            {
                if (lowerButtons[playerIndex][(int)lb.Yat].IsEnabled && mostDie == 5)
                {
                    return lowerButtons[playerIndex][(int)lb.Yat];
                }
                // YachtSides after Yatchside category filled must be played in upper section if available
                if (mostDie > 3 && upperButtons[playerIndex][highDie - 1].IsEnabled)
                {
                    return upperButtons[playerIndex][highDie - 1];
                }
                if (ls)
                {
                    return lowerButtons[playerIndex][(int)lb.Ls];
                }
                if (fh)
                {
                    return lowerButtons[playerIndex][(int)lb.Fh];
                }
                if (ss)
                {
                    return lowerButtons[playerIndex][(int)lb.Ss];
                }
                if (mostDie == 4 && lowerButtons[playerIndex][(int)lb.Foak].IsEnabled)
                {
                    return lowerButtons[playerIndex][(int)lb.Foak];
                }
                if (mostDie == 3)
                {
                    if (lowerButtons[playerIndex][(int)lb.Toak].IsEnabled && 
                        (totalScore > 24 || !upperButtons[playerIndex][highDie - 1].IsEnabled) )
                    {
                        return lowerButtons[playerIndex][(int)lb.Toak];
                    }
                    else if (upperButtons[playerIndex][highDie - 1].IsEnabled)
                    {
                        return upperButtons[playerIndex][highDie - 1];
                    }
                }
                if (mostDie == 2)
                {
                    if (highDie < 4 || diceCount[1] == 2 || diceCount[2] == 2 || diceCount[3] == 2)
                    {
                        int lowPair = Array.IndexOf(new int[] { diceCount[1], diceCount[2], diceCount[3] }, 2);
                        if (totalScore > 21 && lowerButtons[playerIndex][(int)lb.Chance].IsEnabled)
                        {
                            return lowerButtons[playerIndex][(int)lb.Chance];
                        }
                        else if (upperButtons[playerIndex][lowPair].IsEnabled)  // use lowest pair available
                        {
                            return upperButtons[playerIndex][lowPair];
                        }
                        else if(upperButtons[playerIndex][highDie - 1].IsEnabled)
                        {
                            return upperButtons[playerIndex][highDie - 1];
                        }
                    }
                    else
                    {
                        if (totalScore >= 19 && lowerButtons[playerIndex][(int)lb.Chance].IsEnabled)
                        {
                            return lowerButtons[playerIndex][(int)lb.Chance];
                        }
                        else if (upperButtons[playerIndex][0].IsEnabled)
                        {
                            return upperButtons[playerIndex][0];
                        }
                    }
                }
                Button[] desparation = new Button[] {
                    upperButtons[playerIndex][0],
                    lowerButtons[playerIndex][(int)lb.Chance],
                    upperButtons[playerIndex][1],
                    upperButtons[playerIndex][2],
                    lowerButtons[playerIndex][(int)lb.Yat],
                    lowerButtons[playerIndex][(int)lb.Ls],
                    lowerButtons[playerIndex][(int)lb.Fh],
                    lowerButtons[playerIndex][(int)lb.Foak],
                    upperButtons[playerIndex][3],
                    upperButtons[playerIndex][4],
                    upperButtons[playerIndex][5],
                    lowerButtons[playerIndex][(int)lb.Ss],
                    lowerButtons[playerIndex][(int)lb.Toak],
                };
                foreach (Button b in desparation)
                {
                    if (b.IsEnabled)
                    {
                        return b;
                    }
                }
                throw new Exception("You failed to select any buttons!!!");
            }
            else 
            {
                throw new Exception($"roll is a bad number: {roll}");
            }
        }

        private bool lsPossible(int[] diceCount)    // ss used but have 2 chances at ls still
        {
            if ((lowerButtons[playerIndex][(int)lb.Ls].IsEnabled) && 
                (diceCount[2] > 0 && diceCount[3] > 0 && diceCount[4] > 0 && diceCount[5] > 0))
            {
                return true;
            }
            return false;
        }

        private void uncheckOne(int die)
        {
            for (int i = 0; i < 6; ++i)
            {
                if (currentDice[i] == die)
                {
                    boxes[i].IsChecked = false;
                    return;
                }
            }
        }

        (int, int, int, bool, bool, bool) getHand(int[] diceCount)
        {
            int max = diceCount.Max();
            int high = 0;
            int total = 0;

            for (int i = 1; i < 7; ++i)
            {
                total += i * diceCount[i];
                if (diceCount[i] == max) high = i;
            }
            bool fh = (scores[playerIndex][(int)sc.Fh].Text == "25") && lowerButtons[playerIndex][(int)lb.Fh].IsEnabled;
            bool ss = (scores[playerIndex][(int)sc.Ss].Text == "30") && lowerButtons[playerIndex][(int)lb.Ss].IsEnabled;
            bool ls = (scores[playerIndex][(int)sc.Ls].Text == "40") && lowerButtons[playerIndex][(int)lb.Ls].IsEnabled;
            return (max, high, total, fh, ss, ls);
        }

        void keepThese(int[] keeper)
        {
            for (int i = 0; i < 5; ++ i)
            {
                if (keeper.Contains(currentDice[i]))
                {
                    boxes[i].IsChecked = true;
                }
                else
                {
                    boxes[i].IsChecked = false;
                }
            }
        }

        private int[] GetDiceCount()
        {
            int[] result = { 0, 0, 0, 0, 0, 0, 0 }; // index 0 = filler for readability
            for (int i = 0; i < 5; ++i)
            {
                result[currentDice[i]]++;   // increment the count at the index that die reads
            }
            return result;
        }

        private void ResetDie()
        {
            roll = 1; 

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
