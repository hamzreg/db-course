﻿namespace WineSales.Models
{
    public class Wine
    {
        public int ID { get; set; }
        public string Kind { get; set; }
        public string Color { get; set; }
        public string Sugar { get; set; }
        public double Volume { get; set; }
        public double Alcohol { get; set; }
        public int Aging { get; set; }
    }
}
