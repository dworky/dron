using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace dron
{
    class Connection
    {
        public DispatcherTimer SenderTimer { get; set; }
        public DispatcherTimer ReceiverTimer { get; set; }
        UdpClient udpSender;
        UdpClient udpReceiver;
        const string IpAdressSender = "192.168.1.1";
        const string IpAdressReceiver = "224.1.1.1";
        const int SenderPort = 5556;
        const int ReceiverPort = 5554;
        IControl[] controlDevices;      
        public Device CurrentDevice { get; set; }

        public bool ControlActivated { get; set; }
        private bool takeOff = false;
        public bool Calibration { get; set; }
        public bool Land { get; set; } = false;

        public bool TakeOff
        {
            get { return takeOff; }
            set { takeOff = value; }
        }


        public Connection(IControl[] devices, Device currentDevice)
        {
            SenderTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(30)};
            SenderTimer.Tick += senderTimer_Tick;
            ReceiverTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(30)};
            ReceiverTimer.Tick += receiverTimer_Tick;
            controlDevices = devices;
            CurrentDevice = currentDevice;
            ControlActivated = true;
        }

        private void receiverTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (udpReceiver.Available <= 0) return;
                var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, ReceiverPort);
                var buffer = udpReceiver.Receive(ref remoteIpEndPoint);
                if (buffer != null)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(buffer));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
            }
        }

        private void senderTimer_Tick(object sender, EventArgs e)
        {
            if (ControlActivated)
            {
                IControl device = controlDevices[(int) CurrentDevice];
                device.Refresh();
                SendCommand(Instructions.MakeCommandPCMD(1, device.Roll, device.Pitch, device.Gaz, device.Yaw));
            }
            else
            {
                SendCommand(Instructions.CurrentCommand);
                if(takeOff)
                    Instructions.CurrentCommand = Instructions.MakeCommandREF(290718208);
                if (Land)
                {
                    Instructions.CurrentCommand = Instructions.MakeCommandREF(290717696);
                }
                if (Calibration)
                {
                    Instructions.CurrentCommand = Instructions.MakeCommandCalibrate();
                }
            }
        }
    


        public void SendCommand(Byte[] command)
        {
         //   Console.WriteLine(Encoding.UTF8.GetString(command));
            udpSender.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, SenderPort));
        }

        public void Start()
        {
            if (udpSender == null)
            {
                udpSender = new UdpClient(SenderPort);
               // udpSender.Connect(IpAdressSender, SenderPort);
            }
            if (udpReceiver == null)
            {
                udpReceiver = new UdpClient(ReceiverPort) { MulticastLoopback = true };
                udpReceiver.JoinMulticastGroup(IPAddress.Parse(IpAdressReceiver));
                var command = Encoding.ASCII.GetBytes("message");
                udpReceiver.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, ReceiverPort)); 
            }
        }

        public void Stop()
        {
            if(SenderTimer.IsEnabled)
                SenderTimer.Stop();
            if(ReceiverTimer.IsEnabled)
                ReceiverTimer.Stop();
        }

        public void CloseUdp()
        {
            udpReceiver?.Close();
            udpSender?.Close();
        }
    }
}