using System;
using System.Threading;
using System.Diagnostics;
using Windows.Devices.WiFi;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Text;
using nanoFramework.Hardware.Esp32;
using Windows.Devices.Gpio;
using System.Collections;


namespace esptest0
{

    public class Program
    {
        // Set the SSID & Password to your local WiFi network
        /*const string MYSSID = "AndroidAP7914";
        const string MYPASSWORD = "lqbk3422";
        const string IP_ADDR = "192.168.43.61";*/

        const string MYSSID = "WiFimodem-BAB4";
        const string MYPASSWORD = "gjz4gjzntz";
        const string IP_ADDR = "192.168.0.13";
        const int TCP_PORT = 8001;
        // static Command commands = new Command();
        //   [Flags]
        //   enum Command {GetData, SetData, Disconnect, None, SayHello }
        static Ledcolor ledcolor = Ledcolor.Off;
        static GpioPin _redLED = GpioController.GetDefault().OpenPin(2);
        static GpioPin _greenLED = GpioController.GetDefault().OpenPin(0);
        static GpioPin _blueLED = GpioController.GetDefault().OpenPin(4);

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
                Thread.Sleep(10000);
                
                Socket tcpclnt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Console.WriteLine("Created tcp client socket");
                // need an IPEndPoint from that one above
                
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP_ADDR), TCP_PORT);
                Console.WriteLine("Created endpoint to server");
                //   TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");
                tcpclnt.Connect(ep);
                Console.WriteLine("Connected to server");



                using (SslStream stream = new SslStream(tcpclnt))
                {
                    Console.WriteLine("Authenticating server");
                    
                    stream.SslVerification = SslVerification.NoVerification;
                   
                   stream.AuthenticateAsClient(IP_ADDR, SslProtocols.Tls12);


                    Command command = Command.None;
                    Ledcolor ledcolor = Ledcolor.Off;

                    //Initiate GPIO controller
                    //var gpioController = GpioController.GetDefault();
                    // Open connections for LED pins

                    // Define GPIO pin mode
                    _redLED.SetDriveMode(GpioPinDriveMode.Output);
                    _greenLED.SetDriveMode(GpioPinDriveMode.Output);
                    _blueLED.SetDriveMode(GpioPinDriveMode.Output);
                    Configuration.SetPinFunction(25, DeviceFunction.ADC1_CH8);

                    while (true)
                    {
                        // setup buffer to read data from socket
                        command = Command.None;
                        byte[] buffer = new byte[1024];

                        // trying to read from socket
                        // int bytes = tcpclnt.Receive(buffer);
                        int bytes = stream.Read(buffer, 0, buffer.Length);
                         Debug.WriteLine($"Read {bytes} bytes");

                        if (bytes > 0)
                        {
                            // we have data!
                            // output as string
                            Debug.WriteLine(new String(Encoding.UTF8.GetChars(buffer)));

                            if (Command.Disconnect.Name.Equals(new String(Encoding.UTF8.GetChars(buffer))))
                            {
                                command = Command.Disconnect;
                            }
                            else if (Command.None.Name.Equals(new String(Encoding.UTF8.GetChars(buffer))))
                            {
                                command = Command.None;
                            }
                            else if (Command.GetData.Name.Equals(new String(Encoding.UTF8.GetChars(buffer))))
                            {
                                command = Command.GetData;
                            }
                            else if (Command.SetData.Name.Equals(new String(Encoding.UTF8.GetChars(buffer))))
                            {
                                command = Command.SetData;
                            }
                            else if (Command.SayHello.Name.Equals(new String(Encoding.UTF8.GetChars(buffer))))
                            {
                                command = Command.SayHello;
                            }

                        }

                        switch (command.Value)
                        {
                            case 0://DISCONNNECT
                                tcpclnt.Close();
                                Thread.Sleep(Timeout.Infinite);
                                break;
                            case 1://SAYHELLO
                                Console.Write("Transmitting hello world : ");
                                buffer = Encoding.UTF8.GetBytes("Hello world !\r\n");

                                //tcpclnt.Send(buffer);
                                stream.Write(buffer,0, buffer.Length);
                                stream.Flush();
                                Debug.WriteLine($"Send {buffer.Length} bytes");
                                break;
                            case 2://GETDATA

                                buffer = Encoding.UTF8.GetBytes(Gpio.IO25.ToString());

                                tcpclnt.Send(buffer);

                                Debug.WriteLine($"Send {buffer.Length} bytes");
                                break;
                            case 3://SETDATA
                                buffer = new byte[1024];

                                // trying to read from socket
                                bytes = tcpclnt.Receive(buffer);

                                if (bytes > 0)
                                {
                                    Debug.WriteLine(new String(Encoding.UTF8.GetChars(buffer)));
                                    controlLED(new String(Encoding.UTF8.GetChars(buffer)));
                                }
                                break;
                            case 4: //NONE
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("message:" + ex.Message);
                Console.WriteLine("stack:" + ex.StackTrace);
            }

            Thread.Sleep(Timeout.Infinite);
        }

        private static void controlLED(string command)
        {
            if (Ledcolor.Red.Name.Equals(command))
            {
                ledcolor = Ledcolor.Red;
                _greenLED.Write(GpioPinValue.Low);
                _blueLED.Write(GpioPinValue.Low);
                _redLED.Write(GpioPinValue.High);
                Console.WriteLine(ledcolor.Name);
            }
            else if (Ledcolor.Green.Name.Equals(command))
            {
                ledcolor = Ledcolor.Green;
                _redLED.Write(GpioPinValue.Low);
                _blueLED.Write(GpioPinValue.Low);
                _greenLED.Write(GpioPinValue.High);
                Console.WriteLine(ledcolor.Name);
            }
            else if (Ledcolor.Blue.Name.Equals(command))
            {
                ledcolor = Ledcolor.Blue;
                _redLED.Write(GpioPinValue.Low);
                _greenLED.Write(GpioPinValue.Low);
                _blueLED.Write(GpioPinValue.High);
                Console.WriteLine(ledcolor.Name);
            }
            else if (Ledcolor.Magenta.Name.Equals(command))
            {
                ledcolor = Ledcolor.Magenta;
                _greenLED.Write(GpioPinValue.Low);
                _redLED.Write(GpioPinValue.High);
                _blueLED.Write(GpioPinValue.High);

                Console.WriteLine(ledcolor.Name);
            }
            else if (Ledcolor.Cyan.Name.Equals(command))
            {
                ledcolor = Ledcolor.Cyan;
                _greenLED.Write(GpioPinValue.High);
                _redLED.Write(GpioPinValue.Low);
                _blueLED.Write(GpioPinValue.High);
                Console.WriteLine(ledcolor.Name);
            }
            else if (Ledcolor.Yellow.Name.Equals(command))
            {
                ledcolor = Ledcolor.Yellow;
                _blueLED.Write(GpioPinValue.Low);
                _greenLED.Write(GpioPinValue.High);
                _redLED.Write(GpioPinValue.High);
                Console.WriteLine(ledcolor.Name);
            }
            else
            {
                ledcolor = Ledcolor.Off;
                _redLED.Write(GpioPinValue.Low);
                _greenLED.Write(GpioPinValue.Low);
                _blueLED.Write(GpioPinValue.Low);
                Console.WriteLine(ledcolor.Name);
            }

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
    public class Command
    {
        public static Command Disconnect { get; } = new Command(0, "Disconnect");
        public static Command SayHello { get; } = new Command(1, "SayHello");
        public static Command GetData { get; } = new Command(2, "GetData");
        public static Command SetData { get; } = new Command(3, "SetData");
        public static Command None { get; } = new Command(4, "None");

        public string Name { get; private set; }
        public int Value { get; private set; }

        private Command(int val, string name)
        {
            Value = val;
            Name = name;
        }

    }

    public class Ledcolor
    {
        public static Ledcolor Red { get; } = new Ledcolor(0, "Red");
        public static Ledcolor Blue { get; } = new Ledcolor(1, "Blue");
        public static Ledcolor Green { get; } = new Ledcolor(2, "Green");
        public static Ledcolor Magenta { get; } = new Ledcolor(3, "Magenta");
        public static Ledcolor Cyan { get; } = new Ledcolor(4, "Cyan");
        public static Ledcolor Yellow { get; } = new Ledcolor(5, "Yellow");
        public static Ledcolor White { get; } = new Ledcolor(6, "White");
        public static Ledcolor Off { get; } = new Ledcolor(7, "Off");
        public string Name { get; private set; }
        public int Value { get; private set; }

        private Ledcolor(int val, string name)
        {
            Value = val;
            Name = name;
        }

    }
}
