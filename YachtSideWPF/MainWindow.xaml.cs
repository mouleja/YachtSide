using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        UserControl _currentUC;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            RulesPopup.Visibility = Visibility.Collapsed;
            AboutPopup.IsOpen = false;
            CurrentControl = new SetupGame(this);
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public UserControl CurrentControl
        { 
            get { return _currentUC; }
            set
            {
                _currentUC = value;
                //MainUserControlPanel.Content = _currentUC;
                NotifyPropertyChanged("CurrentControl");
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
            if (RulesPopup.Visibility == Visibility.Visible)
            {
                MainUserControlPanel.Visibility = Visibility.Visible;
                RulesPopup.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainUserControlPanel.Visibility = Visibility.Collapsed;
                RulesPopup.Visibility = Visibility.Visible;
            }
        }

        private void CloseRulesButton_Click(object sender, RoutedEventArgs e)
        {
            RulesPopup.Visibility = Visibility.Collapsed;
            MainUserControlPanel.Visibility = Visibility.Visible;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (AboutPopup.IsOpen) AboutPopup.IsOpen = false;
            else AboutPopup.IsOpen = true;
        }

        private void CloseAboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutPopup.IsOpen = false;
        }
    }
}