using System;
using System.Collections.Generic;
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
    public partial class ScoresUserControl : UserControl
    {
        UserControl _priorUC;
        MainWindow _parent;

        public ScoresUserControl(MainWindow parent, UserControl priorUC)
        {
            InitializeComponent();
            _priorUC = priorUC;
            _parent = parent;
        }

        private void CloseScoresButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.CurrentControl = _priorUC;
        }
    }
}
