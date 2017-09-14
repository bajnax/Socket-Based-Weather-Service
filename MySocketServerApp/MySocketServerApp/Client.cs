using System;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Net.Sockets;

namespace MySocketServerApp
{
    class Client
    {
        public TcpClient clientSocket = default(TcpClient);
        public void communication()
        {
            try
            {
                while ((true))
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[10025];
                    if (networkStream.CanRead)
                    {
                        StringBuilder myCompleteMessage = new StringBuilder();
                        int numberOfBytesRead = 0;
                        Weather weather = new Weather();

                        // Incoming message may be larger than the buffer size.
                        do
                        {
                            numberOfBytesRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                            myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(bytesFrom, 0, numberOfBytesRead));
                        }
                        while (networkStream.DataAvailable);

                        string dataFromClient = myCompleteMessage.ToString();
                        weather = stringToWeather(dataFromClient);

                        Console.WriteLine(" >> Data from client: " + Environment.NewLine 
                            + "City - " + weather.City + Environment.NewLine 
                            + "Temperature - " + weather.Temperature + Environment.NewLine
                            + "Rain - " + weather.Rain);

                        foreach (var c in Program.clients)
                        {
                            c.writeMsg(dataFromClient);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
                    }
                }
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                clientSocket.Close();
                Console.WriteLine(ex.ToString());
            }           
        }

        public void writeMsg(String dataFromClient)
        {
            try
            {
                NetworkStream networkStream = clientSocket.GetStream();
                if (networkStream.CanWrite)
                {
                    string serverResponse = dataFromClient;
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                }
                else
                {
                    Console.WriteLine("Sorry.  You cannot write to this NetworkStream.");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Weather stringToWeather(string xmlString)
        {
            if (xmlString.Equals(null))
                return null;

            var xmlSer = new XmlSerializer(typeof(Weather));
            Weather weather;

            using (TextReader tReader = new StringReader(xmlString))
                weather = (Weather)xmlSer.Deserialize(tReader);

            return weather;
        }

    }
}
