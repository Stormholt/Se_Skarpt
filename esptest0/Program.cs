using System;
using System.Threading;
using System.Diagnostics;
using Windows.Devices.WiFi;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using nanoFramework.Hardware.Esp32;


namespace esptest0
{
     
    public class Program
    {
        // Set the SSID & Password to your local WiFi network
        const string MYSSID = "AndroidAP7914";
        const string MYPASSWORD = "lqbk3422";
        //const string MYSSID = " WiFimodem-BAB4";
        //const string MYPASSWORD = "gjz4gjzntz";

        public static void Main()
        {
            try
            {
                // Get the first WiFI Adapter
                WiFiAdapter wifi = WiFiAdapter.FindAllAdapters()[0];
                // Set up the AvailableNetworksChanged event to pick up when scan has completed
                wifi.AvailableNetworksChanged += Wifi_AvailableNetworksChanged;

                Console.WriteLine("starting WiFi scan");
                wifi.ScanAsync();

                Thread.Sleep(30000);



                Socket tcpclnt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
             
                // need an IPEndPoint from that one above
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.43.61"), 8001);
                //   TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");
                tcpclnt.Connect(ep);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");
                Console.Write("Transmitting hello world : ");
                byte[] buffer = Encoding.UTF8.GetBytes("Hello world !\r\n");

                tcpclnt.Send(buffer);

                Debug.WriteLine($"Send {buffer.Length} bytes");

                Configuration.SetPinFunction(25, DeviceFunction.ADC1_CH8);
                

                buffer = Encoding.UTF8.GetBytes(Gpio.IO25.ToString());

                tcpclnt.Send(buffer);

                Debug.WriteLine($"Send {buffer.Length} bytes");

                Configuration.SetPinFunction(36, DeviceFunction.ADC1_CH0);
                Configuration.SetPinFunction(39, DeviceFunction.ADC1_CH3);
                Configuration.SetPinFunction(34, DeviceFunction.ADC1_CH6);


                //Gpio.IO36 = 1;

                //// setup buffer to read data from socket
                //buffer = new byte[1024];

                //// trying to read from socket
                //int bytes = tcpclnt.Receive(buffer);

                //Debug.WriteLine($"Read {bytes} bytes");

                //if (bytes > 0)
                //{
                //    // we have data!
                //    // output as string
                //    Debug.WriteLine(new String(Encoding.UTF8.GetChars(buffer)));
                //}
                Thread.Sleep(50000);

                tcpclnt.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("message:" + ex.Message);
                Console.WriteLine("stack:" + ex.StackTrace);
            }

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Event handler for when WiFi scan completes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Wifi_AvailableNetworksChanged(WiFiAdapter sender, object e)
        {
            Console.WriteLine("Wifi_AvailableNetworksChanged - get report");

            // Get Report of all scanned WiFi networks
            WiFiNetworkReport report = sender.NetworkReport;

            // Enumerate though networks looking for our network
            foreach (WiFiAvailableNetwork net in report.AvailableNetworks)
            {
                // Show all networks found
              //  Console.WriteLine($"Net SSID :{net.Ssid},  BSSID : {net.Bsid},  rssi : {net.NetworkRssiInDecibelMilliwatts.ToString()},  signal : {net.SignalBars.ToString()}");

                // If its our Network then try to connect
                if (net.Ssid == MYSSID)
                {
                    // Disconnect in case we are already connected
                    sender.Disconnect();

                    // Connect to network
                    WiFiConnectionResult result = sender.Connect(net, WiFiReconnectionKind.Automatic, MYPASSWORD);

                    // Display status
                    if (result.ConnectionStatus == WiFiConnectionStatus.Success)
                    {
                        Console.WriteLine("Connected to Wifi network");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Error {result.ConnectionStatus.ToString()} connecting o Wifi network");
                    }
                }
            }
        }
    }
 
}
