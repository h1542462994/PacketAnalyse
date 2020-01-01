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
using PacketAnalyse.Core.Filters;

namespace PacketAnalyse
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkListenerGroup group;
        NetworkObservableCollection obs;
        HashSet<IPAddress> banedLocalIP = new HashSet<IPAddress>();
        private bool status = false;
        bool isLoaded = false;
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
            isLoaded = true;
            RefreshLocalIP();
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
            obs.Clear();
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                var r = (RadioButton)sender;
                var tag = (string)r.Tag;

                if (tag == "A1")
                {
                    obs.Filters.TypeFilter = new InternetTypeFilter(InternetType.All);
                }
                else if (tag == "A2")
                {
                    obs.Filters.TypeFilter = new InternetTypeFilter(InternetType.Inner);
                }
                else if (tag == "A3")
                {
                    obs.Filters.TypeFilter = new InternetTypeFilter(InternetType.Outer);
                } 
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            { 
                var result = 0;
                foreach (CheckBox item in StackPanel1.Children)
                {
                    result += int.Parse((string)item.Tag) * ((bool)item.IsChecked ? 1:0) ;
                }
                obs.Filters.ProtocalFilter = new InternetProtocalFilter((ProtocalFilterOption)result);
            }
        }

        private void RefreshLocalIP()
        {
            HashSet<IPAddress> lists = new HashSet<IPAddress>();
            foreach (CheckBox item in StackPanelLocalIP.Children)
            {
                var ip = (IPAddress)item.Tag;
                if (item.IsChecked == false)
                {
                    lists.Add(ip);
                }
                item.Click -= Item_Click;
            }
            StackPanelLocalIP.Children.Clear();
            foreach (var item in NetworkHelper.GetIpv4s())
            {
                lists.Add(item);
            }

            foreach (var item in lists)
            {
                CheckBox c = new CheckBox() { Tag = item, Content = item.ToString() ,Margin = new Thickness(5)};
                c.Click += Item_Click;
                StackPanelLocalIP.Children.Add(c);
                if (banedLocalIP.Contains(item))
                {
                    c.IsChecked = false;
                } else
                {
                    c.IsChecked = true;
                }
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                var c = (CheckBox)sender;
                var tag = (IPAddress)c.Tag;
                if (c.IsChecked == true)
                {
                    banedLocalIP.Remove(tag);
                } else
                {
                    banedLocalIP.Add(tag);
                }
                obs.Filters.LocalIPFilter = new LocalIPFilter(banedLocalIP.ToArray());
            }
        }

        private void ButtonRefreshIP_Click(object sender, RoutedEventArgs e)
        {
            RefreshLocalIP();
        }
    }
}
