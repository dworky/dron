using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace dron.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Helpers.IControl[] controlDevices;
        DispatcherTimer timerMainLoop;


        private enum Device
        {
            GamePad = 0,
            Keyboard = 1,
            Kinect = 2
        }

        Device currentDevice;

        public MainWindow()
        {
            InitializeComponent();
            controlDevices = new Helpers.IControl[3];
            controlDevices[0] = new Helpers.GamePad(PadStatusChanged);
            controlDevices[1] = new Helpers.Keyboard();
            controlDevices[2] = new Helpers.Kinect();
            currentDevice = Device.Keyboard;
            timerMainLoop = new DispatcherTimer();
            timerMainLoop.Interval = TimeSpan.FromMilliseconds(30);
            timerMainLoop.Tick += timerMainLoop_Tick;


        }


        private void b_WifiConnection_Click(object sender, RoutedEventArgs e)
        {
            if (img_WifiDisconnected.IsVisible)
            {
                tB_WifiConnection.Text = "Rozłącz";
                img_WifiConnected.Visibility = Visibility.Visible;
                img_WifiDisconnected.Visibility = Visibility.Collapsed;
                b_Start.IsEnabled = true;
                b_Land.IsEnabled = true;
                //UDP client ctor
                timerMainLoop.Start();
            }
            else
            {
                tB_WifiConnection.Text = "Połącz";
                img_WifiConnected.Visibility = Visibility.Collapsed;
                img_WifiDisconnected.Visibility = Visibility.Visible;
                b_Start.IsEnabled = false;
                b_Land.IsEnabled = false;
                timerMainLoop.Stop();
                //UDP client close
            }


        }


        private void rB_Control_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as System.Windows.Controls.RadioButton;
            switch ((string)radioButton.Content)
            {
                case "Pad":
                    currentDevice = Device.GamePad;
                    break;
                case "Klawiatura":
                    currentDevice = Device.Keyboard;
                    break;
                case "Kinect":
                    currentDevice = Device.Kinect;
                    break;
            }
        }

        private void s_Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            l_Speed.Content = "Szybkość manewrów: " + (int)s_Speed.Value + "%";
            Helpers.Instructions.Speed = (float)s_Speed.Value / 100;
            Console.WriteLine(Helpers.Instructions.Speed);
        }

        private void PadStatusChanged(bool status)
        {
            if (status)
            {
                img_PadConnected.Visibility = Visibility.Visible;
                img_PadDisconnected.Visibility = Visibility.Collapsed;
            }
            else
            {
                img_PadConnected.Visibility = Visibility.Collapsed;
                img_PadDisconnected.Visibility = Visibility.Visible;
            }
        }

        private void timerMainLoop_Tick(object sender, EventArgs e)
        {
            Helpers.IControl device = controlDevices[(int)currentDevice];
            device.Refresh();
            var command = Helpers.Instructions.MakeCommandPCMD(1 , device.Roll, device.Pitch, device.Gaz, device.Yaw);
            //now send by udp
            Console.WriteLine("Wysłano pakiet");
        }

        private void b_Start_Click(object sender, RoutedEventArgs e)
        {
            var command = Helpers.Instructions.MakeCommandREF(290718208);
            //now send by udp
        }

        private void b_Land_Click(object sender, RoutedEventArgs e)
        {
            var command = Helpers.Instructions.MakeCommandREF(290717696);
            //now send by udp
        }

        private void b_Info_Click(object sender, RoutedEventArgs e)
        {
            AboutBox info = new AboutBox();
            info.Show();
        }
    }
}
