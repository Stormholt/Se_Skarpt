using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SQLiteDatabase;
using System.Collections.Generic;



namespace SeSkarpApplikation
{
    class SeSkarptServer
    {
        //const string IP_ADDR = "192.168.43.61"; // IP med Ajs hotspot
      //  const string IP_ADDR = "192.168.0.13"; // IP hos Ajs Tobaksvejen 2c
        const string IP_ADDR = "192.168.43.15"; // IP med Jeppe hotspot
        const int TCP_PORT = 8001;

        private static Mutex mutex = new Mutex(); //Mutex to protect the tcp read and write
        public enum Command { GetData, SetData, Disconnect, None, SayHello }
        public enum Ledcolor { Red, Blue, Green, Magenta, Cyan, Yellow, White, Off }

        public Command command { get; set; }
        public Ledcolor ledcolor { get; set; }

        private TcpListener listener = null;
        private Socket client = null;
        private ASCIIEncoding asen = null;

        public sqLitedatabase databaseObject = null; 

        public bool connected { get; private set; }
        public SeSkarptServer()
        {
            command = Command.None;
            ledcolor = Ledcolor.Off;
            listener = new TcpListener(IPAddress.Parse(IP_ADDR), TCP_PORT); 
            asen = new ASCIIEncoding();
            databaseObject = new sqLitedatabase();
            connected = false;
        }

      
        public void SendLEDCommand(Ledcolor ledcolor)
        {
            this.ledcolor = ledcolor;
            mutex.WaitOne();//Locks mutex
            client.Send(asen.GetBytes(ledcolor.ToString())); // sends a bite array with the LED color name as string.
            mutex.ReleaseMutex(); // Opens mutex
            Console.WriteLine($"LED color {ledcolor.ToString()} sent \r");
        }

        public void SendCommand(Command command)
        {
            this.command = command;
            mutex.WaitOne();
            client.Send(asen.GetBytes(command.ToString()));
            mutex.ReleaseMutex();
            Console.WriteLine($"Command {command.ToString()} sent");
            this.command = Command.None;
        }

        public void ReadStringData()
        {
            byte[] buffer = new byte[100];
            mutex.WaitOne();
            int k = client.Receive(buffer);
            mutex.ReleaseMutex();
            Console.Write("Received: ");
            for (int i = 0; i < k; i++) //Converts each element of the byte array and converts to char.
                Console.Write(Convert.ToChar(buffer[i]));
            Console.Write("\n");
        }

        public void ReadSensorData()
        {
            int temp = 0;
            int light = 0;
            string msg;
            byte[] lightBuffer = new byte[2];
            byte[] tempBuffer = new byte[2];

            mutex.WaitOne();
            //Recieve light value
            int k = client.Receive(lightBuffer);
            Console.Write("Received : ");
            msg = Encoding.ASCII.GetString(lightBuffer);
            light = Int32.Parse(msg);
            Console.Write($"{light} lm ");
            //Receive temperature value
            k = client.Receive(tempBuffer);
            msg = Encoding.ASCII.GetString(tempBuffer);
            temp = Int32.Parse(msg);
            Console.WriteLine($"{temp} C");

            Write2Database(temp, light); // Write the new values to database
           
            mutex.ReleaseMutex();
            
        }

        // starts server and waits until client is connected.
        public void ConnectDevice()
        {
            if (listener.LocalEndpoint != null)
            {
                try
                {
                    Console.WriteLine("Starting server.....");
                    listener.Start();
                    Console.WriteLine("TCP server online at {0}", listener.LocalEndpoint);
                    Console.WriteLine("Waiting for a connection.....");
                    client = listener.AcceptSocket();
                    Console.WriteLine("Connection accepted from " + client.RemoteEndPoint);
                    connected = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error..... " + e.StackTrace);
                }
            }
        }

        //Disconnect device and a bit of clean up.
        public void DisconnectDevice()
        {

            SendCommand(Command.Disconnect);
            connected = false;
            mutex.WaitOne();
            client.Close();
            mutex.ReleaseMutex();
            Console.WriteLine("Device disconnected");
            listener.Stop();
            Console.WriteLine("TCP server offline...");
        }

        //Write sensor values to database.
        private void Write2Database(int temp, int light)
        {
            databaseObject.OpenConnection();
            databaseObject.sendData(temp, light);
            databaseObject.CloseConnection();
  
        }

    }
}
