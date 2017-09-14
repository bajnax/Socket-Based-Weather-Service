using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Threading;


namespace MyClientSocketApp
{
    public partial class Form1 : Form
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream;
        Thread mt;
        IPAddress lhost = IPAddress.Parse("127.0.0.1");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                msg("Client Started");
                clientSocket.Connect(lhost, 8888);
                label1.Text = "Client Socket Program - Server Connected ...";
                serverStream = clientSocket.GetStream();

                this.mt = new Thread(new ThreadStart(this.readMsg));

                // Start the thread
                this.mt.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Weather weather = new Weather();
            weather.City = textBox2.Text;
            weather.Temperature = double.Parse(textBox3.Text);
            weather.Rain = double.Parse(textBox4.Text);

            String data = weatherToString(weather);

            byte[] outStream = Encoding.ASCII.GetBytes(data);
            if (serverStream.CanWrite)
            {
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
            else
            {
                Console.WriteLine("Sorry.  You cannot write to this NetworkStream.");
            }

            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox2.Focus();
        }

        public void msg(string mesg)
        {
            textBox1.Text = textBox1.Text + Environment.NewLine + " >> " + mesg;

        }

        private delegate void RefreshTextBox(String s);

        private void readMsg()
        {
            while (true)
            {
                byte[] inStream = new byte[10025];
                StringBuilder myCompleteMessage = new StringBuilder();
                int numberOfBytesRead = 0;
                if (serverStream.CanRead)
                {
                    do
                    {
                        numberOfBytesRead = serverStream.Read(inStream, 0, inStream.Length);
                        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(inStream, 0, numberOfBytesRead));
                    }
                    while (serverStream.DataAvailable);

                    string returndata = myCompleteMessage.ToString();

                    Weather weather = new Weather();

                    //convert returndata string ro weather object here by deserialization
                    weather = stringToWeather(returndata);

                    returndata = "Data from Client:" + Environment.NewLine 
                        + " >> City - " + weather.City + Environment.NewLine 
                        + " >> Temperature - " + weather.Temperature + Environment.NewLine 
                        + " >> Rain - " + weather.Rain + Environment.NewLine;

                    if (this.InvokeRequired)
                    {
                        RefreshTextBox d = new RefreshTextBox(msg);
                        Invoke(d, returndata);
                    }
                    else
                    {
                        msg(returndata);
                    }

                }
                else
                {
                    Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
                }
                Thread.Sleep(100);
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

        public string weatherToString(Weather weather)
        {
            if (weather == null)
                return null;

            var strBuilder = new StringBuilder();
            var itemType = weather.GetType();
            new XmlSerializer(itemType).Serialize(new StringWriter(strBuilder), weather);
            return strBuilder.ToString();
        }

    }
}
