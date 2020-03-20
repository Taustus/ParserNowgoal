using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ParserNowgoal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BrowserWindow _bw;
        List<Match> _matches;
        Thread _browserThr;
        CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();
            _matches = new List<Match>();
            _bw = new BrowserWindow(this);
            _bw.Show();
        }

        public bool AddRow(Match match)
        {
            var add = false;
            add = !_matches.Any(m => m.ID == match.ID);
            if (add)
            {
                Dispatcher.Invoke(() =>
                {
                    _matches.Add(match);
                    table_dg.ItemsSource = _matches;
                    table_dg.Items.Refresh();
                });
            }
            return add;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _bw.Close();
        }

        public void UpdateStatus(string status)
        {
            Dispatcher.Invoke(() => status_lbl.Content = status);
        }

        public void UpdateName(string name)
        {
            chosenSite_lbl.Dispatcher.Invoke(new Action(() => chosenSite_lbl.Content = name));
        }

        private void start_btn_Click(object sender, RoutedEventArgs e)
        {
            start_btn.IsEnabled = false;
            stop_btn.IsEnabled = true;
            _cts = new CancellationTokenSource();
            List<object> lst = new List<object> { this, _cts.Token };
            _browserThr = new Thread(new ParameterizedThreadStart(_bw.Algorithm));
            _browserThr.Start(lst);
        }

        private void stop_btn_Click(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
            _cts.Dispose();
            _browserThr.Interrupt();
            stop_btn.IsEnabled = false;
            start_btn.IsEnabled = true;
        }
    }
}
