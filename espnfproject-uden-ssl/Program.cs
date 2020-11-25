using System;
//Nanoframework specific classes
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




namespace espnfproject
{

    public class Program
    {
        // Set the SSID & Password to your local WiFi network
        // Ajs mobil hotspot
        /*const string MYSSID = "AndroidAP7914";
        const string MYPASSWORD = "lqbk3422";
        const string IP_ADDR = "192.168.43.61";*/

        // Ajs hjemme wifi
        const string MYSSID = "WiFimodem-BAB4";
        const string MYPASSWORD = "gjz4gjzntz";
        const string IP_ADDR = "192.168.0.13";

        const int TCP_PORT = 8001;

        const int photoresistor_PIN = 0;
        const int LM35_PIN = 3;
        const int LEDR_PIN = 2;
        const int LEDG_PIN = 0;
        const int LEDB_PIN = 4;
        
        public static void Main()
        {
            try
            {
                // Get the first WiFI Adapter
                WiFiAdapter wifi = WiFiAdapter.FindAllAdapters()[0];
                // Set up the AvailableNetworksChanged event to pick up when scan has completed
                wifi.AvailableNetworksChanged += Wifi_AvailableNetworksChanged;

                Debug.WriteLine("starting WiFi scan");
                wifi.ScanAsync();
                Thread.Sleep(10000);
                
                Socket tcpclnt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Debug.WriteLine("Created tcp client socket");
                // need an IPEndPoint from that one above
                
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP_ADDR), TCP_PORT);
                Debug.WriteLine("Created endpoint to server");
                //TcpClient tcpclnt = new TcpClient();
                Debug.WriteLine("Connecting.....");
                tcpclnt.Connect(ep);
                Debug.WriteLine("Connected to server");

               
                Command command = Command.None;
                Ledcolor ledcolor = Ledcolor.Off;

                RGBLED rgbled = new RGBLED(GpioController.GetDefault().OpenPin(LEDR_PIN),
                                             GpioController.GetDefault().OpenPin(LEDG_PIN),
                                             GpioController.GetDefault().OpenPin(LEDB_PIN));

                AdcController adcController = AdcController.GetDefault();
                adcController.ChannelMode = AdcChannelMode.SingleEnded;    // ADC value comes from a single point 
                Photoresistor photoresistor = new Photoresistor( adcController.OpenChannel(photoresistor_PIN));// ADC channel 0 - Photoresistor
                LM35 lm35 = new LM35(adcController.OpenChannel(LM35_PIN));// ADC channel 3 - LM35 Themistor

                byte[] buffer;


                while (true)
                {
                    // setup buffer to read data from socket
                    command = Command.None;
                    buffer = new byte[1024];

                    // trying to read from socket
                     int bytes = tcpclnt.Receive(buffer);

                    if (bytes > 0)
                    {
                        Debug.WriteLine(new String(Encoding.UTF8.GetChars(buffer)));
                        command = command.FindCommand(new String(Encoding.UTF8.GetChars(buffer)));
                    }
                    buffer = new byte[1024];
                    switch (command.Value)
                    {
                        case 0://DISCONNNECT
                            tcpclnt.Close();
                            Thread.Sleep(Timeout.Infinite);
                            break;
                        case 1://SAYHELLO
                            Debug.Write("Transmitting hello world : ");
                            buffer = Encoding.UTF8.GetBytes("Hello world !\r\n");
                            tcpclnt.Send(buffer);
                            break;
                        case 2://GETDATA
                            string msg = $"Light level: {photoresistor.CalculateLumen()} Lumen, Temperature: {lm35.CalculateCelsius()}C";
                            buffer = Encoding.UTF8.GetBytes(msg);
                            tcpclnt.Send(buffer);
                            break;
                        case 3://SETDATA
                            bytes = tcpclnt.Receive(buffer);
                            if (bytes > 0)
                            {
                                rgbled.controlLED(Ledcolor.Off.FindLedcolor( new String(Encoding.UTF8.GetChars(buffer))));
                            }
                            break;
                        case 4: //NONE
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("message:" + ex.Message);
                Debug.WriteLine("stack:" + ex.StackTrace);
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
            Debug.WriteLine("Wifi_AvailableNetworksChanged - get report");

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
    }




    /*ATTEMPT AT PROGRAMMING DHT22 protocol.  */

    /*    public class Dhtreading
        {
            bool TimedOut;
            bool IsValid;
            double Temperature;
            double Humidity;
            int RetryCount;


        }*/

    /*    public class Dht22
        {
            GpioPin pin;

            public Dht22()
            {
                this.pin = null;
            }
            public Dht22(GpioPin pin, GpioPinDriveMode inputReadMode)
            {
                this.pin = pin;
                this.pin.SetDriveMode(inputReadMode);
            }

            Dhtreading InternalGetReading()
            {

            }

            Dhtreading CalculateValues(ArrayList<bitArray>, )

        }*/



    /*ulong curTime;
                 ulong prevTime;
                 HighResTimer timer = new HighResTimer();
                _lm35.SetDriveMode(GpioPinDriveMode.Output);
                _lm35.Write(GpioPinValue.High);
                Thread.Sleep(10);//10 milliseconds
                _lm35.Write(GpioPinValue.Low);
                Thread.Sleep(18);
                _lm35.Write(GpioPinValue.High);
                _lm35.SetDriveMode(GpioPinDriveMode.Input);
                prevTime = HighResTimer.GetCurrent();
                curTime = HighResTimer.GetCurrent();
                while(curTime - prevTime< 40){
                    curTime = HighResTimer.GetCurrent();
                }
                if (_lm35.Read() == GpioPinValue.Low)
                {
                    Debug.WriteLine("Response from DHT was send at first attempt");
                    for( int i = 0; i<40; i++)
                    {
                        

                    }
                
                }
                else
                {
                     Debug.WriteLine(" NO!! Response from DHT ");
                }*/
}
/*  Configuration.SetPinFunction(21, DeviceFunction.I2C1_CLOCK);
                Configuration.SetPinFunction(22, DeviceFunction.I2C1_DATA);

                Bmp280 bmp280 = new Bmp280();
                bmp280.Initialize();
                while (bmp280.IsMeasuring()) { }
                
                if (bmp280.Read())
                {   
                    float temp = bmp280.TemperatureInCelcius;
                    float pressure = bmp280.PressureInPa;
                    Debug.WriteLine($"temp = {temp}, pressure = {pressure}");
                }
                else { Debug.WriteLine("Failed to read bmp280"); }
*/
/*I2cConnectionSettings settings = new I2cConnectionSettings(0x77);
settings.BusSpeed = I2cBusSpeed.StandardMode;
settings.SharingMode = I2cSharingMode.Exclusive;
Debug.WriteLine("I2C settings is set");*/
/*  Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);
  Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);*/

/*I2cController i2cController = I2cController.GetDefault();
      Debug.WriteLine("I2C Controller is set");
      i2cDevice = I2cDevice.FromId("i2cBus", new I2cConnectionSettings(BMP280_ADDR)); ;
      Debug.WriteLine("I2C Device is set");*/
/*     I2cConnectionSettings settings = new I2cConnectionSettings(22, BMP280_ADDR);
     I2cDevice i2cDevice = new I2cDevice(settings);
     i2cDevice.Read(buffer);
     Debug.WriteLine(new String(Encoding.UTF8.GetChars(buffer)));*/