using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MySocketServerApp
{
    class Program
    {
        static public List<Client> clients = new List<Client>();
        static public TcpListener serverSocket;

        static void Main(string[] args)
        {

            try
            {
                IPAddress lhost = IPAddress.Parse("127.0.0.1");
                //TcpListener 
                serverSocket = new TcpListener(lhost, 8888);
                TcpClient clientSocket = default(TcpClient);
                serverSocket.Start();
                Console.WriteLine(" >> Server Started");
                Console.WriteLine(" >> Waiting for clients to connect ");
                while (true)
                {
                    clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine(" >> Accept connection from a client");

                    Client c = new Client();
                    clients.Add(c);

                    // Insert this client into a collection

                    c.clientSocket = clientSocket;

                    Thread mt = new Thread(new ThreadStart(c.communication));
                    mt.Start();
                }
                serverSocket.Stop();
                Console.WriteLine(" >> exit");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                serverSocket.Stop();
                Console.WriteLine(" >> exit");
                Console.ReadLine();
                throw e;
            }

            
        }

    }
}
