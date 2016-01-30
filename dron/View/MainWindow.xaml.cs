using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;


namespace dron.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Device currentDevice;
        IControl[] controlDevices;                     
        Connection connection;
        private bool isSkeletonTracked;


        public MainWindow()
        {
            InitializeComponent();
            controlDevices = new IControl[3];
            controlDevices[0] = new GamePad(PadStatusChanged);
            controlDevices[1] = new Keyboard();
            controlDevices[2] = new Kinect();
            currentDevice = Device.Keyboard;
            connection = new Connection(controlDevices, currentDevice);
            this.Closing += OnWindowClosing;
            ((Kinect) controlDevices[2]).SkeletonTracked += KinectSkeletonTracked;
            DataContext = controlDevices[2];
        }
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            ((Kinect)controlDevices[2]).Stop();
            connection?.Stop();
            connection?.CloseUdp();
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
                b_CalibrateHorizontally.IsEnabled = true;
                connection.Start();              
            }
            else
            {
                tB_WifiConnection.Text = "Połącz";
                img_WifiConnected.Visibility = Visibility.Collapsed;
                img_WifiDisconnected.Visibility = Visibility.Visible;
                b_Start.IsEnabled = false;
                b_Land.IsEnabled = false;
                b_CalibrateHorizontally.IsEnabled = false;
                connection.Stop();
            }
        }

        private void rB_Control_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            switch ((string)radioButton.Content)
            {
                case "Pad":
                    if(connection != null) connection.CurrentDevice = Device.GamePad;
                    break;
                case "Klawiatura":
                    if (connection != null) connection.CurrentDevice = Device.Keyboard;
                    break;
                case "Kinect":
                    if (connection != null && isSkeletonTracked) connection.CurrentDevice = Device.Kinect;
                    break;
            }
        }

        private void s_Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            l_Speed.Content = "Szybkość manewrów: " + (int)s_Speed.Value + "%";
            Instructions.Speed = (float)s_Speed.Value / 100;
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

        private void KinectSkeletonTracked(object sender, EventArgs eventArgs)
        {
            isSkeletonTracked = (bool) sender;
            if(isSkeletonTracked)
            {
                img_KinectConnected.Visibility = Visibility.Visible;
                img_KinectDisconnected.Visibility = Visibility.Collapsed;
            }
            else
            {
                img_KinectConnected.Visibility = Visibility.Collapsed;
                img_KinectDisconnected.Visibility = Visibility.Visible;
                //if (connection?.CurrentDevice == Device.Kinect)
                //    connection.CurrentDevice = Device.Keyboard;
            }
        }

        private void b_Start_Click(object sender, RoutedEventArgs e)
        {
            if (!connection.SenderTimer.IsEnabled)
                connection.SenderTimer.Start();
            if (!connection.ReceiverTimer.IsEnabled)
                connection.ReceiverTimer.Start();

            var tB = sender as ToggleButton;
            if ((bool)tB.IsChecked)
            {
                if (connection != null)
                {
                    Instructions.CurrentCommand = Instructions.MakeCommandREF(290718208);
                    connection.ControlActivated = false;
                    b_Land.IsEnabled = false;
                    b_CalibrateHorizontally.IsEnabled = false;
                    connection.Start();
                    connection.TakeOff = true;
                }
            }
            else
            {
                Instructions.CurrentCommand = Instructions.MakeCommandPCMD(flag: 0);
                connection.ControlActivated = true;
                b_Land.IsEnabled = true;
                b_CalibrateHorizontally.IsEnabled = true;
                connection.TakeOff = false;

            }
        }

        private void b_Land_Click(object sender, RoutedEventArgs e)
        {           
            var tB = sender as ToggleButton;
            if ((bool)tB.IsChecked)
            {
                if (connection != null)
                {
                    Instructions.CurrentCommand = Instructions.MakeCommandREF(290717696);
                    connection.ControlActivated = false;
                    b_Start.IsEnabled = false;
                    b_CalibrateHorizontally.IsEnabled = false;
                    connection.Land = true;
                }                
            }
            else
            {
                connection.Land = false;
                Instructions.CurrentCommand = Instructions.MakeCommandPCMD(flag: 0);
                connection.ControlActivated = true;
                b_Start.IsEnabled = true;
                b_CalibrateHorizontally.IsEnabled = true;
            }
        }

        private void b_CalibrateHorizontally_Click(object sender, RoutedEventArgs e)
        {
            if (!connection.SenderTimer.IsEnabled)
                connection.SenderTimer.Start();
            if (!connection.ReceiverTimer.IsEnabled)
                connection.ReceiverTimer.Start();
            var tB = sender as ToggleButton;
            if ((bool)tB.IsChecked)
            {
                if (connection != null)
                {
                   // connection.Calibration = true;
                    Instructions.CurrentCommand = Instructions.MakeCommandCalibrate();
                    connection.ControlActivated = false;
                    b_Start.IsEnabled = false;
                    b_Land.IsEnabled = false;
                }
            }
            else
            {
              //  connection.Calibration = false;
                Instructions.CurrentCommand = Instructions.MakeCommandPCMD(flag: 0);
                connection.ControlActivated = true;
                b_Start.IsEnabled = true;
                b_Land.IsEnabled = true;
            }
        }

        private void b_SequenceReset_Click(object sender, RoutedEventArgs e)
        {
            Instructions.Sequence = 0;
        }

        private void b_Info_Click(object sender, RoutedEventArgs e)
        {
            AboutBox info = new AboutBox();
            info.Show();
        }      
    }
}
