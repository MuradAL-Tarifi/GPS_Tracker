using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GPS.Domain.Models
{
    public class History
    {
        public long Id { get; set; }
        public long VehicleId { get; set; }
        public string IMEI { get; set; }
        public DateTime GPSDate { get; set; }
        public int AlertCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Alitude { get; set; }
        public int Speed { get; set; }
        public int Direction { get; set; }
        public int Odometer { get; set; }
        public int HDOP { get; set; }
        public int SatelliteCount { get; set; }
        public bool Ignition { get; set; }
        public decimal? Temperature1 { get; set; }
        public decimal? Temperature2 { get; set; }
        public decimal? Temperature3 { get; set; }
        public decimal? Temperature4 { get; set; }
        public decimal? Humidity1 { get; set; }
        public decimal? Humidity2 { get; set; }
        public decimal? Humidity3 { get; set; }
        public decimal? Humidity4 { get; set; }
        public int? TotalWeight { get; set; }
        public int? WeightVolt { get; set; }
        public int? Door1 { get; set; }
        public int? Door2 { get; set; }
        public bool? Sos { get; set; }
        public bool? SosPassenger { get; set; }
        public bool? Seat { get; set; }
        public int? NumberOfPassengers { get; set; }
    }
}
