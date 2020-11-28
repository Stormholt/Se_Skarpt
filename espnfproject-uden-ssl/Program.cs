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
using ESPnanoFrameworkApp;

namespace espnfproject
{

    public class Program
    {
       
        
        public static void Main()
        {
            try
            {
                
                
               

               
                

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
                            rgbled.controlLED(Ledcolor.Off);
                            tcpclnt.Close();
                            Thread.Sleep(Timeout.Infinite);
                            break;
                        case 1://SAYHELLO
                            Debug.Write("Transmitting : Hello world !");
                            buffer = Encoding.UTF8.GetBytes("Hello world !");
                            tcpclnt.Send(buffer);
                            break;
                        case 2://GETDATA
                            string msg = $"{photoresistor.CalculateLumen()} ";
                            buffer = Encoding.UTF8.GetBytes(msg);
                            tcpclnt.Send(buffer);
                             msg = $"{lm35.CalculateCelsius()}";
                            buffer = new byte[1];
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