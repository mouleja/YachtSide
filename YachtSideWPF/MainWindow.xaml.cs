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
        UserControl _currentUC;

        public MainWindow()
        {
            InitializeComponent();

            _currentUC = new SetupGame(this);
            MainUserControlPanel.Content = _currentUC;
        }

        public UserControl CurrentControl
        { 
            get { return _currentUC; }
            set
            {
                _currentUC = value;
                MainUserControlPanel.Content = _currentUC;
            }
        }

        private void NewGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentControl = new SetupGame(this);
        }

        private void ScoresMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UserControl lastControl = CurrentControl;
            CurrentControl = new ScoresUserControl(this, lastControl);
        }

        private void RulesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainUserControlPanel.Visibility = Visibility.Collapsed;
            RulesPopup.Visibility = Visibility.Visible;
        }

        private void CloseRulesButton_Click(object sender, RoutedEventArgs e)
        {
            RulesPopup.Visibility = Visibility.Hidden;
            MainUserControlPanel.Visibility = Visibility.Visible;
        }
    }
}