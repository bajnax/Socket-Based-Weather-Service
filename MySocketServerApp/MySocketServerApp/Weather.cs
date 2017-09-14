using System;

namespace MySocketServerApp
{
    [Serializable()]
    public class Weather
    {
        public string City;
        public double Temperature;
        public double Rain;
    }
}