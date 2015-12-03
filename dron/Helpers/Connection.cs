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
        DispatcherTimer senderTimer; 
        DispatcherTimer receiverTimer; 
        UdpClient udpSender;
        UdpClient udpReceiver;
        const string ipAdress = "192.168.1.1";
        const int senderPort = 5556;
        const int receiverPort = 5554;
        IControl[] controlDevices;      
        public Device CurrentDevice { get; set; }

        public bool ControlActivated { get; set; }


        public Connection(IControl[] devices, Device currentDevice)
        {                       
            senderTimer = new DispatcherTimer();
            senderTimer.Interval = TimeSpan.FromMilliseconds(30);
            senderTimer.Tick += senderTimer_Tick;
            receiverTimer = new DispatcherTimer();
            receiverTimer.Interval = TimeSpan.FromMilliseconds(30);
            receiverTimer.Tick += receiverTimer_Tick;
            controlDevices = devices;
            CurrentDevice = currentDevice;
            ControlActivated = true;
        }

        private void receiverTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (udpReceiver.Available <= 0) return;
                var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, receiverPort);
                var buffer = udpReceiver.Receive(ref remoteIpEndPoint);
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

        private void senderTimer_Tick(object sender, EventArgs e)
        {
            if (ControlActivated)
            {
                IControl device = controlDevices[(int)CurrentDevice];
                device.Refresh();
                SendCommand(Instructions.MakeCommandPCMD(1, device.Roll, device.Pitch, device.Gaz, device.Yaw));
            }
            else SendCommand(Instructions.CurrentCommand);           
        }


        public void SendCommand(Byte[] command)
        {
            if(udpSender != null)            
            udpSender.Send(command, command.Length); 
            //jak nie zadziała , new IPEndPoint(IPAddress.Broadcast, senderPort));
        }

        public void Start()
        {
            if (udpSender == null && udpReceiver == null)
            {
                udpSender = new UdpClient();
                udpReceiver = new UdpClient();

                udpSender.Connect(ipAdress, senderPort);
                senderTimer.Start();
            }
        }

        public void Stop()
        {
            if (udpSender != null && udpReceiver != null)
            {
                senderTimer.Stop();
                udpSender.Close();
                udpReceiver.Close();
                udpSender = null;
                udpReceiver = null;
            }            
        }
    }
}

//Zakomentowany twój kod

//receiverClient = new UdpClient(ReceivePort) { MulticastLoopback = true };
//var multiCastGroupAddress = IPAddress.Parse("224.1.1.1");
//receiverClient.JoinMulticastGroup(multiCastGroupAddress);
//var command = Encoding.ASCII.GetBytes("message");
//receiverClient.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, ReceivePort));
// timer.Start();

//command = Encoding.ASCII.GetBytes("AT*CONFIG=\\\"general:navdata_demo\\\",\\\"TRUE\\\"\\r");

//udpClient.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, Port));
// receiverClient.Send(command, command.Length, new IPEndPoint(IPAddress.Broadcast, Port));

// "AT*CONFIG=\"general:navdata_demo\",\"TRUE\"\\r"