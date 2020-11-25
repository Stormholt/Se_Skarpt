/*
Inspired greatly by:
https://www.codeproject.com/Articles/1415/Introduction-to-TCP-client-server-in-C
https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
https://docs.microsoft.com/en-us/dotnet/api/system.net.security.sslstream?view=net-5.0
*/
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;



public class tcpserver0
{

    //const string IP_ADDR = "192.168.43.61"; // IP med Ajs hotspot
     const string IP_ADDR = "192.168.0.13"; // IP hos Ajs Tobaksvejen 2c

    const int TCP_PORT = 8001;

    [Flags]
    enum Command { GetData, SetData, Disconnect, None, SayHello }
    [Flags]
    enum LED { Red, Blue, Green, Magenta, Cyan, Yellow, White, Off }

    public static void Main()
    {
        try
        {   /* Initializes the Listener */

            // string ipadr = (string)asr.GetValue("IpAdresse", typeof(string));

            TcpListener listener = new TcpListener(IPAddress.Parse(IP_ADDR), TCP_PORT);
            listener.Start();
            Console.WriteLine("TCP server online at {0}", listener.LocalEndpoint);
            Console.WriteLine("Waiting for a connection.....");


            Socket client = listener.AcceptSocket();
            Console.WriteLine("Connection accepted from " + client.RemoteEndPoint);
            ASCIIEncoding asen = new ASCIIEncoding();
            Command command = Command.None;
            LED ledcolor = LED.Off;
            bool foundflag = false;


            while (client.Connected == true)
            {
                byte[] b = new byte[100];
                int k = 0;
                client.Send(asen.GetBytes(command.ToString()));

                Console.WriteLine("Command sent");
                switch (command)
                {
                    case Command.SayHello:
                        b = new byte[100];
                        k = client.Receive(b);
                        Console.WriteLine("Received...");
                        for (int i = 0; i < k; i++)
                            Console.Write(Convert.ToChar(b[i]));
                        command = Command.None;
                        break;
                    case Command.Disconnect:
                        command = Command.None;
                        client.Close();
                        listener.Stop();
                        Environment.Exit(0);
                        break;
                    case Command.GetData:
                        b = new byte[100];
                        k = client.Receive(b);
                        Console.WriteLine("Received...");
                        for (int i = 0; i < k; i++)
                            Console.Write(Convert.ToChar(b[i]));
                        Console.Write("\n");
                        command = Command.None;
                        break;
                    case Command.SetData:
                        do
                        {
                            Console.WriteLine("Please type valid LED color ( Red, Blue, Green, Magenta, Cyan, Yellow, White, Off): ");
                            string ledinput = Console.ReadLine();
                            foreach (string color in Enum.GetNames(typeof(LED)))
                            {
                                if (color.Equals(ledinput))
                                {
                                    ledcolor = (LED)Enum.Parse(typeof(LED), color);
                                    foundflag = true;
                                }
                            }
                        } while (foundflag != true);
                        foundflag = false;

                        client.Send(asen.GetBytes(ledcolor.ToString()));

                        Console.WriteLine("LED color sent \r");
                        command = Command.None;
                        break;
                    case Command.None:
                        do
                        {
                            Console.WriteLine("Please type valid command (GetData, SetData, Disconnect, None, SayHello): ");
                            string userInput = Console.ReadLine();
                            foreach (string c in Enum.GetNames(typeof(Command)))
                            {
                                if (c.Equals(userInput))
                                {
                                    command = (Command)Enum.Parse(typeof(Command), c);
                                    foundflag = true;
                                }
                            }
                        } while (foundflag != true);
                        foundflag = false;

                        break;
                }

            }

        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }


}


