using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SharpDX.Text;
using Encoding = System.Text.Encoding;

namespace dron.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        IControl[] controlDevices;
        DispatcherTimer timerMainLoop,timer;
        private UdpClient udpClient;
        private UdpClient receiverClient;
        private const int Port = 5556;
        private const int ReceivePort = 5554;

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
            controlDevices = new IControl[3];
            controlDevices[0] = new GamePad(PadStatusChanged);
            controlDevices[1] = new Keyboard();
            controlDevices[2] = new Kinect();
            currentDevice = Device.Keyboard;
            timerMainLoop = new DispatcherTimer();
            timerMainLoop.Interval = TimeSpan.FromMilliseconds(30);
            timerMainLoop.Tick += timerMainLoop_Tick;
            //timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
            //timer.Tick += udpReceive_Tick;

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
                udpClient = new UdpClient();

                //receiverClient = new UdpClient(ReceivePort) { MulticastLoopback = true };
                //var multiCastGroupAddress = IPAddress.Parse("224.1.1.1");
                //receiverClient.JoinMulticastGroup(multiCastGroupAddress);
                //var command = Encoding.ASCII.GetBytes("message");
                //receiverClient.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, ReceivePort));
               // timer.Start();
            }
            else
            {
                Instructions.Sequence = 0;
                tB_WifiConnection.Text = "Połącz";
                img_WifiConnected.Visibility = Visibility.Collapsed;
                img_WifiDisconnected.Visibility = Visibility.Visible;
                b_Start.IsEnabled = false;
                b_Land.IsEnabled = false;
                timerMainLoop.Stop();
                //UDP client close
                udpClient.Close();
               //receiverClient.Close();
               // timer.Stop();
            }
        }

        private void rB_Control_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
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
            Instructions.Speed = (float)s_Speed.Value / 100;
            Console.WriteLine(Instructions.Speed);
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
            IControl device = controlDevices[(int) currentDevice];
            device.Refresh();
            var command = Instructions.MakeCommandPCMD(1, device.Roll, device.Pitch, device.Gaz, device.Yaw);
            udpClient.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, Port));
     
        }

        private void udpReceive_Tick(object sender, EventArgs e)
        {
            try
            {
                if (receiverClient.Available <= 0) return;
                var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, ReceivePort);
                var buffer = receiverClient.Receive(ref remoteIpEndPoint);
                if (buffer != null)
                {
                    Console.WriteLine(Encoding.ASCII.GetString(buffer));
                }
                //receiverClient.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
            }
        }
        private void b_Start_Click(object sender, RoutedEventArgs e)
        {
            if(!timerMainLoop.IsEnabled)
                timerMainLoop.Start();
            var command = Instructions.MakeCommandREF(290718208);
            udpClient.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, Port));
        }

        private void b_Land_Click(object sender, RoutedEventArgs e)
        {
            var command = Instructions.MakeCommandREF(290717696);
            udpClient.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, Port));
        }

        private void b_Info_Click(object sender, RoutedEventArgs e)
        {
            AboutBox info = new AboutBox();
            info.Show();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var command = Encoding.ASCII.GetBytes($"AT*FTRIM={Instructions.Sequence}\r");
            if(udpClient.Client.Connected)
                udpClient.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, Port));

            //command = Encoding.ASCII.GetBytes("AT*CONFIG=\\\"general:navdata_demo\\\",\\\"TRUE\\\"\\r");

            //udpClient.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, Port));
           // receiverClient.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, Port));

            // "AT*CONFIG=\"general:navdata_demo\",\"TRUE\"\\r"

        }
    }
}
