using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacketAnalyse
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Core.NetworkListener listener;
        ObservableCollection<Core.IPDatagramScope> data = new ObservableCollection<Core.IPDatagramScope>();
        public bool status = false;
        public bool Status { get => status; set
            {
                this.status = value;
                if (value == true)
                {
                    ButtonContiune.IsEnabled = false;
                    ButtonPause.IsEnabled = true;
                    listener.Contiune();
                } else
                {
                    ButtonContiune.IsEnabled = true;
                    ButtonPause.IsEnabled = false;
                    listener.Pause();
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
            listener = new Core.NetworkListener(Core.LocalIPHelper.Get());
            listener.InternetDataReceived += Listener_InternetDataReceived;
            DataGridMain.DataContext = data;
        }

        private void Listener_InternetDataReceived(Core.NetworkListener sender, Core.InternetDataReceivedEventArgs<Core.IPDatagram> e)
        {
            Dispatcher.Invoke(() =>
            {
                data.Add(e.Data.Scope);
            });
        }

        private void Button_Click_Contiune(object sender, RoutedEventArgs e)
        {
            Status = true;
        }

        private void Button_Click_Pause(object sender, RoutedEventArgs e)
        {
            Status = false;
        }

        private void Button_Click_Clear(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }
    }
}
