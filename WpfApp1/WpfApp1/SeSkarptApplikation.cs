using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SQLiteDatabase;



namespace SeSkarpApplikation
{
    class SeSkarptServer
    {
        //const string IP_ADDR = "192.168.43.61"; // IP med Ajs hotspot
      //  const string IP_ADDR = "192.168.0.13"; // IP hos Ajs Tobaksvejen 2c
        const string IP_ADDR = "192.168.43.15"; // IP med Jeppe hotspot
        const int TCP_PORT = 8001;
        public const int THREAD_SLEEP_TIME_SEC = 10;

        private static Mutex mutex = new Mutex();
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

        public void ThreadMethod()
        {
            Console.WriteLine("Hello from Thread");
            while (true)
            {
                if (connected == true)
                {
                    Console.WriteLine("Thread sampling");
                    ReadSensorData();
                    Thread.Sleep(THREAD_SLEEP_TIME_SEC * 1000);

                    Console.WriteLine("Thread not sampling");
                    Thread.Sleep(THREAD_SLEEP_TIME_SEC * 1000);
                }
            }
        }
        public void SendLEDCommand(Ledcolor ledcolor)
        {
            this.ledcolor = ledcolor;
            mutex.WaitOne();
            client.Send(asen.GetBytes(ledcolor.ToString()));
            mutex.ReleaseMutex();
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
            for (int i = 0; i < k; i++)
                Console.Write(Convert.ToChar(buffer[i]));
            Console.Write("\n");
        }

        public void ReadSensorData()
        {
            int temp = 0;
            int light = 0;
            byte[] buffer = new byte[10];
            mutex.WaitOne();
            int k = client.Receive(buffer);
            Console.Write("Received : ");
            light =  Convert.ToInt32(buffer[0]);
            Console.Write($"{light} ");
            buffer = new byte[10];
            k = client.Receive(buffer);
            temp = Convert.ToInt32(buffer[0]);
            Console.Write($"{temp} ");
            Console.Write("\n");
            Write2Database(temp, light);
            mutex.ReleaseMutex();
        }

        public void ConnectDevice()
        {
            if (listener.LocalEndpoint != null)
            {
                try
                {
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

        private void Write2Database(int temp, int light)
        {
            databaseObject.OpenConnection();
            databaseObject.sendData(temp, light);
            databaseObject.CloseConnection();
  
        }

    }
}
