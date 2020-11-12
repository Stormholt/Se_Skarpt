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

            // Socket client = listener.AcceptSocket();
            TcpClient client = listener.AcceptTcpClient();

            //  Console.WriteLine("Connection accepted from " + client.RemoteEndPoint);
            using (SslStream stream = new SslStream(client.GetStream(),false))
            {
                stream.AuthenticateAsServer(null, clientCertificateRequired: false, checkCertificateRevocation: true);

                // Set timeouts for the read and write to 5 seconds.
                stream.ReadTimeout = 5000;
                stream.WriteTimeout = 5000;

                ASCIIEncoding asen = new ASCIIEncoding();
                Command command = Command.None;
                LED ledcolor = LED.Off;
                bool foundflag = false;


                while (client.Connected == true)
                {
                    byte[] b = new byte[100];
                 //   int k = 0;
                    // client.Send(asen.GetBytes(command.ToString()));
                    stream.Write(asen.GetBytes(command.ToString()));
                    Console.WriteLine("Command sent");
                    switch (command)
                    {
                        case Command.SayHello:
                            b = new byte[100];
                            // k = client.Receive(b);
                            Console.WriteLine(ReadMessage(stream));
                           /* Console.WriteLine("Received...");
                            for (int i = 0; i < k; i++)
                                Console.Write(Convert.ToChar(b[i]));*/
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
                            /*k = client.Receive(b);
                            Console.WriteLine("Received...");
                            for (int i = 0; i < k; i++)
                                Console.Write(Convert.ToChar(b[i]));*/
                            Console.WriteLine(ReadMessage(stream));
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

                            //client.Send(asen.GetBytes(ledcolor.ToString()));
                            stream.Write(asen.GetBytes(ledcolor.ToString()));
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
        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }

    static string ReadMessage(SslStream sslStream)
    {
        // Read the  message sent by the client.
        // The client signals the end of the message using the
        // "<EOF>" marker.
        byte[] buffer = new byte[2048];
        StringBuilder messageData = new StringBuilder();
        int bytes = -1;
        do
        {
            // Read the client's test message.
            bytes = sslStream.Read(buffer, 0, buffer.Length);

            // Use Decoder class to convert from bytes to UTF8
            // in case a character spans two buffers.
            Decoder decoder = Encoding.UTF8.GetDecoder();
            char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
            decoder.GetChars(buffer, 0, bytes, chars, 0);
            messageData.Append(chars);
            // Check for EOF or an empty message.
            if (messageData.ToString().IndexOf("<EOF>") != -1)
            {
                break;
            }
        } while (bytes != 0);

        return messageData.ToString();
    }

}


