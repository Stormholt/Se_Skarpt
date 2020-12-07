using System;
//Nanoframework specific classes
using System.Threading;
using System.Diagnostics;

//Classes created for this project
using SeSkarptEnumClasses;
using ESPnanoFrameworkApp;

namespace espnfproject
{

    public class Program
    {

        public static void Main()
        {
            try
            {
                ESPclient client = new ESPclient(); 
                client.ConnectToWifi();
                client.ConnectToServer();

                while (true)
                {
                    client.command = Command.None;

                    client.ReadCommand();

                    switch (client.command.Value)
                    {
                        case 0://DISCONNNECT
                            client.rgbled.controlLED(Ledcolor.Off); // turn off LED
                            client.tcpclnt.Close(); //Close TCP Connection.
                            Thread.Sleep(Timeout.Infinite);
                            break;
                        case 1://SAYHELLO
                            client.SendHello();
                            break;
                        case 2://GETDATA
                            client.SendData();
                            break;
                        case 3://SETDATA
                            client.ReadRGBcolor();
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

            
        }
    }
}