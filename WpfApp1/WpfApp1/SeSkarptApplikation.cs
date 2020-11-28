using System;

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
        // const string IP_ADDR = "192.168.0.13"; // IP hos Ajs Tobaksvejen 2c
        const string IP_ADDR = "192.168.43.15"; // IP med Jeppe hotspot
        const int TCP_PORT = 8001;


        public enum Command { GetData, SetData, Disconnect, None, SayHello }
        public enum Ledcolor { Red, Blue, Green, Magenta, Cyan, Yellow, White, Off }

        public Command command {get; set ;} 
        public Ledcolor ledcolor { get; set; }

        private TcpListener listener = null;
        private Socket client = null;
        private ASCIIEncoding asen = null;

        public sqLitedatabase databaseObject = null; 


        public SeSkarptServer()
        {
            command = Command.None;
            ledcolor = Ledcolor.Off;
            listener = new TcpListener(IPAddress.Parse(IP_ADDR), TCP_PORT);
            asen = new ASCIIEncoding();
            databaseObject = new sqLitedatabase();
        }

        public void SendLEDCommand(Ledcolor ledcolor)
        {
            this.ledcolor = ledcolor;
            client.Send(asen.GetBytes(ledcolor.ToString()));
            Console.WriteLine($"LED color {ledcolor.ToString()} sent \r");
        }

        public void SendCommand(Command command)
        {
            this.command = command;
            client.Send(asen.GetBytes(command.ToString()));
            Console.WriteLine($"Command {command.ToString()} sent");
            this.command = Command.None;
        }

        public void ReadStringData()
        {
            byte[] buffer = new byte[100];
            int k = client.Receive(buffer);
            Console.Write("Received: ");
            for (int i = 0; i < k; i++)
                Console.Write(Convert.ToChar(buffer[i]));
            Console.Write("\n");
        }

        public void ReadSensorData()
        {
            List<int> dataList = new List<int>();
            int temp = 0;
            int light = 0;
            byte[] buffer = new byte[10];
            int k = client.Receive(buffer);
            Console.Write("Received : ");
            light =  Convert.ToInt32(buffer[0]);
            Console.Write($"{light} ");
            buffer = new byte[10];
            dataList = new List<int>();
            k = client.Receive(buffer);
            temp = Convert.ToInt32(buffer[0]);
            Console.Write($"{temp} ");
            Console.Write("\n");
            Write2Database(temp, light);
            
        }

        public void ConnectDevice()
        {
            try
            {
                listener.Start();
                Console.WriteLine("TCP server online at {0}", listener.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");
                client = listener.AcceptSocket();
                Console.WriteLine("Connection accepted from " + client.RemoteEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        public void DisconnectDevice()
        {
            SendCommand(Command.Disconnect);
            client.Close();
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
