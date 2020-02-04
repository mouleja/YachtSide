using System;
using System.Collections.Generic;
using System.Text;

namespace YachtSideWPF
{
    public class ScoreEntry
    {
        public ScoreEntry(string date, int score, string name)
        {
            Date = date;
            Score = score;
            Name = name;
        }

        public string Date { get; set; }
        public int Score { get; set; }
        public string Name { get; set; }
    }
}
