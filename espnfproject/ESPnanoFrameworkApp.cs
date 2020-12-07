//Nanoframework specific classes
using System;
using System.Threading;
using System.Diagnostics;
using Windows.Devices.WiFi;
using Windows.Devices.Adc;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Windows.Devices.Gpio;

//Classes created for this project
using espSensors;
using SeSkarptEnumClasses;
using RGBled;



namespace ESPnanoFrameworkApp
{
    

     class ESPclient
    {


        // Ajs mobil hotspot
        /*const string MYSSID = "AndroidAP7914";
        const string MYPASSWORD = "lqbk3422";
        const string IP_ADDR = "192.168.43.61";*/

        // Ajs hjemme wifi
        /*   const string MYSSID = "WiFimodem-BAB4";
           const string MYPASSWORD = "gjz4gjzntz";
           const string IP_ADDR = "192.168.0.13";*/

        //Jeppe hotspot
        const string MYSSID = "ajsertilguys";
        const string MYPASSWORD = "ajsajsbby";
         const string IP_ADDR = "192.168.43.15";
   

        const int TCP_PORT = 8001;

        const int photoresistor_PIN = 0;
        const int LM35_PIN = 3;
        const int LEDR_PIN = 2;
        const int LEDG_PIN = 0;
        const int LEDB_PIN = 4;

        WiFiAdapter wifi;
        IPEndPoint ep;
        public Socket tcpclnt;

        public Command command;
        public Ledcolor ledcolor;
        public RGBLED rgbled;

        AdcController adcController;
        public Photoresistor photoresistor;
        public LM35 lm35;
        public bool connected { get; private set; }

        public ESPclient()
        {
            command = Command.None;
            Ledcolor ledcolor = Ledcolor.Off;
            rgbled = new RGBLED(GpioController.GetDefault().OpenPin(LEDR_PIN),
                                GpioController.GetDefault().OpenPin(LEDG_PIN),
                                GpioController.GetDefault().OpenPin(LEDB_PIN));
            adcController = AdcController.GetDefault();
            adcController.ChannelMode = AdcChannelMode.SingleEnded;    // ADC value comes from a single point 
            photoresistor = new Photoresistor(adcController.OpenChannel(photoresistor_PIN));// ADC channel 0 - Photoresistor
            lm35 = new LM35(adcController.OpenChannel(LM35_PIN));// ADC channel 3 - LM35 Themistor
            connected = false;
        }
        /// <summary>
        /// Connects to Wifi specified and initliase the TCP socket
        /// </summary>
        public void ConnectToWifi()
        {
            wifi = WiFiAdapter.FindAllAdapters()[0];
            wifi.AvailableNetworksChanged += Wifi_AvailableNetworksChanged;

            Debug.WriteLine("starting WiFi scan");
            wifi.ScanAsync();
            Thread.Sleep(10000);// Waits because the scan is async.

            tcpclnt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Debug.WriteLine("Created tcp client socket");

        }
        /// <summary>
        /// Event handler for when WiFi scan completes
        /// This method was inspired by the wifiScan example by the nanoFramework.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Wifi_AvailableNetworksChanged(WiFiAdapter sender, object e)
        {
            // Get Report of all scanned WiFi networks
            WiFiNetworkReport report = sender.NetworkReport;

            // Enumerate though networks looking for our network
            foreach (WiFiAvailableNetwork net in report.AvailableNetworks)
            {
                // Show all networks found
                //  Debug.WriteLine($"Net SSID :{net.Ssid},  BSSID : {net.Bsid},  rssi : {net.NetworkRssiInDecibelMilliwatts.ToString()},  signal : {net.SignalBars.ToString()}");

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

                        Debug.WriteLine("Connected to Wifi network");
                        break;
                    }
                    else
                    {
                        Debug.WriteLine($"Error {result.ConnectionStatus.ToString()} connecting o Wifi network");
                    }
                }
            }
        }

        public void ConnectToServer()
        {
            try
            {
                ep = new IPEndPoint(IPAddress.Parse(IP_ADDR), TCP_PORT);
                Debug.WriteLine("Created endpoint to server");
                Debug.WriteLine("Connecting.....");
                tcpclnt.Connect(ep);
                Debug.WriteLine("Connected to server");
                connected = true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("message:" + ex.Message);
                Debug.WriteLine("stack:" + ex.StackTrace);
            }
        }

        private byte[] Read() { 
        
            byte[] buffer = new byte[20];
            int bytes = tcpclnt.Receive(buffer);
            Debug.WriteLine(new String(Encoding.UTF8.GetChars(buffer)));
            return buffer;
        }

        public void ReadCommand()
        {
            command = command.FindCommand(new String(Encoding.UTF8.GetChars(Read())));
        }

        public void ReadRGBcolor()
        {
            rgbled.controlLED(Ledcolor.Off.FindLedcolor(new String(Encoding.UTF8.GetChars(Read()))));
        }

        private void Send(byte[] buffer)
        {
            tcpclnt.Send(buffer);
        }

        public void SendData()
        {
            byte[] lightbuffer;
            string msg = $"{photoresistor.CalculateLumen()}";
            Debug.WriteLine(msg);
            lightbuffer = Encoding.UTF8.GetBytes(msg);
            Send(lightbuffer);

            msg = $"{lm35.CalculateCelsius()}";
            Debug.WriteLine(msg);
            byte[] tempbuffer; 
            tempbuffer = Encoding.UTF8.GetBytes(msg);
            Send(tempbuffer);
        }

        public void SendHello()
        {
            byte[] buffer = new byte[10];
            buffer = Encoding.UTF8.GetBytes("Hello world !");
            Send(buffer);
        }

    }
}