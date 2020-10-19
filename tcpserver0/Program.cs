﻿/*
Inspired greatly by:
https://www.codeproject.com/Articles/1415/Introduction-to-TCP-client-server-in-C

*/
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class tcpserver0
{
    public static void Main()
    {
        try
        {
            IPAddress ipAd = IPAddress.Parse("192.168.43.61");
            // use local m/c IP address, and 
            // use the same in the client

            /* Initializes the Listener */
            TcpListener myList = new TcpListener(ipAd, 8001);

            /* Start Listeneting at the specified port */
            myList.Start();

            Console.WriteLine("The server is running at port 8001...");
            Console.WriteLine("The local End point is  :" +
                              myList.LocalEndpoint);
            Console.WriteLine("Waiting for a connection.....");

            Socket s = myList.AcceptSocket();
            Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

            byte[] b = new byte[100];
            int k = s.Receive(b);
            Console.WriteLine("Received...");
            for (int i = 0; i < k; i++)
                Console.Write(Convert.ToChar(b[i]));

            ASCIIEncoding asen = new ASCIIEncoding();
            s.Send(asen.GetBytes("The string was recieved by the server."));
            Console.WriteLine("\nSent Acknowledgement");

            b = new byte[100];
            k = s.Receive(b);
            Console.WriteLine("Received...");
            for (int i = 0; i < k; i++)
                Console.Write(Convert.ToChar(b[i]));

            asen = new ASCIIEncoding();
            s.Send(asen.GetBytes("The string was recieved by the server."));
            Console.WriteLine("\nSent Acknowledgement");
            /* clean up */
            s.Close();
            myList.Stop();

        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }

}
