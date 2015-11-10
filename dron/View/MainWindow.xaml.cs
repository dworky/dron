using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace dron.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Helpers.GamePad gamePad = new Helpers.GamePad(PadStatusChanged);
            var dd = Helpers.Instructions.MakeCommandREF(4);
            var dd1 = Helpers.Instructions.MakeCommandREF(4);
            var dd2 = Helpers.Instructions.MakeCommandREF(4);
            int dsaas = Helpers.Instructions.Sequence;
            var dds2 = Helpers.Instructions.MakeCommandREF(4);
        }


        private void b_WifiConnection_Click(object sender, RoutedEventArgs e)
        {
            if (img_WifiDisconnected.IsVisible)
            {
                tB_WifiConnection.Text = "Rozłącz";
                img_WifiConnected.Visibility = Visibility.Visible;
                img_WifiDisconnected.Visibility = Visibility.Collapsed;
            }
            else
            {
                tB_WifiConnection.Text = "Połącz";
                img_WifiConnected.Visibility = Visibility.Collapsed;
                img_WifiDisconnected.Visibility = Visibility.Visible;
            }


        }


        private void rB_Control_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void s_Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            l_Speed.Content = "Szybkość manewrów: " + (int)s_Speed.Value + "%";
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
    }
}
