using System;
using System.IO;
using System.Xml.Serialization;

namespace MyClientSocketApp
{
    [Serializable()]
    public class Weather
    {
        public string City;
        public double Temperature;
        public double Rain;
    }
}