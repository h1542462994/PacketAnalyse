using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PacketAnalyse.Core;
using System.ComponentModel;

namespace PacketAnalyse
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkListenerGroup group;
        NetworkObservableCollection obs;
        private bool status = false;
        public bool Status { get => status; set
            {
                this.status = value;
                if (value)
                {
                    ButtonContinue.IsEnabled = false;
                    ButtonPause.IsEnabled = true;
                    ButtonFilter.IsEnabled = false;
                    IPSelectBox.IsEnabled = false;
                    group.Start();
                } 
                else
                {
                    ButtonContinue.IsEnabled = true;
                    ButtonPause.IsEnabled = false;
                    ButtonFilter.IsEnabled = true;
                    IPSelectBox.IsEnabled = true;
                    group.Stop();
                }
            }
        }
        private bool isFilterOpen = false;
        public bool IsFilterOpen { get => isFilterOpen; set
            {
                isFilterOpen = value;
                if (value)
                {
                    GridLayer.Visibility = Visibility.Visible;
                } else
                {
                    GridLayer.Visibility = Visibility.Collapsed;
                }
            } 
        }


        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            group = new NetworkListenerGroup();
            obs = new NetworkObservableCollection(group, Dispatcher);
            DataGridMain.DataContext = obs.Scopes;
        }

        private void Button_Click_Continue(object sender, RoutedEventArgs e)
        {
            Status = true;
        }

        private void Button_Click_Pause(object sender, RoutedEventArgs e)
        {
            Status = false;
        }

        private void Button_Click_Clear(object sender, RoutedEventArgs e)
        {
            obs.Scopes.Clear();
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            IsFilterOpen = true;
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                IsFilterOpen = false;
            }
        }
    }
}
