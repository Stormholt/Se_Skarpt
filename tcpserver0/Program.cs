/*
Inspired greatly by:
https://www.codeproject.com/Articles/1415/Introduction-to-TCP-client-server-in-C
https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
*/
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Threading;
/*public class ReceiveHandler
{
    // Size of receive buffer.  
    public const int BufferSize = 1024;

    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];

    // Received data string.
    public StringBuilder sb = new StringBuilder();

    // Client socket.
    public Socket workSocket = null;
}*/
public class tcpserver0
{

    //const string IP_ADDR = "192.168.43.61";
    const string IP_ADDR = "192.168.0.13";
    const int TCP_PORT = 8001;
    // ASCIIEncoding asen = new ASCIIEncoding();
    //public static ManualResetEvent allDone = new ManualResetEvent(false);
    [Flags]
    enum Command { GetData, SetData, Disconnect, None, SayHello }
    [Flags]
    enum LED { Red, Blue, Green, Magenta, Cyan, Yellow, White, Off }
  //  static AppSettingsReader asr = new AppSettingsReader();
    public static void Main()
    {
        try
        {   /* Initializes the Listener */
            
           // string ipadr = (string)asr.GetValue("IpAdresse", typeof(string));

            TcpListener listener = new TcpListener(IPAddress.Parse(IP_ADDR), TCP_PORT);
            listener.Start();
            Console.WriteLine("TCP server online at {0}", listener.LocalEndpoint);
            Console.WriteLine("Waiting for a connection.....");

            Socket s = listener.AcceptSocket();
            Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

            ASCIIEncoding asen = new ASCIIEncoding();
            Command command = Command.None;
            LED ledcolor = LED.Off;
            bool foundflag = false;
            

            while (s.Connected == true)
            {
                byte[] b = new byte[100];
                int k = 0;
                s.Send(asen.GetBytes(command.ToString()));
                Console.WriteLine("Command sent \r");
                switch (command)
                {
                    case Command.SayHello:
                        b = new byte[100];
                        k = s.Receive(b);
                        Console.WriteLine("Received...");
                        for (int i = 0; i < k; i++)
                            Console.Write(Convert.ToChar(b[i]));
                        command = Command.None;
                        break;
                    case Command.Disconnect:
                        command = Command.None;
                        s.Close();
                        listener.Stop();
                        Environment.Exit(0);
                        break;
                    case Command.GetData:
                        b = new byte[100];
                        k = s.Receive(b);
                        Console.WriteLine("Received...");
                        for (int i = 0; i < k; i++)
                            Console.Write(Convert.ToChar(b[i]));
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
                                    ledcolor = (LED) Enum.Parse(typeof(LED), color);
                                    foundflag = true;
                                }
                            }
                        } while (foundflag != true);
                        foundflag = false;
                
                        s.Send(asen.GetBytes(ledcolor.ToString()));
                        
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


