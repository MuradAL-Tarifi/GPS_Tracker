using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class InventorySensorTemperatureModel
    {
        public string GatewayIMEI { get; set; }
        public string Serial { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public bool? IsLowVoltage { get; set; }
        public DateTime GpsDate { get; set; }
        public string Alram { get; set; }
        public string GSMStatus { get; set; }
        public string SensorName { get; set; }
        public long WarehouseId { get; set; }
        public long InventoryId { get; set; }
        public string InventoryName { get; set; }
        public bool HasAnyRecords { get; set; }
        public double LastUpdateMinutes
        {
            get
            {
                return DateTime.Now.Subtract(GpsDate).TotalMinutes;
            }
        }
        public string Color
        {
            get
            {
                if (HasAnyRecords)
                {
                    if (LastUpdateMinutes < 60)
                    {
                        return "bg-light-success";
                    }
                    else if (LastUpdateMinutes >= 60 && LastUpdateMinutes < 120)
                    {
                        return "bg-light-warning";
                    }
                    else
                    {
                        return "bg-light-danger";
                    }
                }
                else
                {
                    return "bg-light-secondary";
                }
            }
        }
        public string BtnColor
        {
            get
            {
                if (HasAnyRecords)
                {
                    if (Temperature > 0 && Humidity > 0)
                    {
                        return "btn btn-info";
                    }
                    else
                    {
                        return "btn btn-danger";
                    }
                }
                else
                {
                    return "btn btn-secondary";
                }
            }
        }
        public bool IsAccepted
        {
            get
            {
                return DateTime.Now.Subtract(GpsDate).TotalMinutes < 120;
            }
        }
        public bool IsHumidityAccepted
        {
            get
            {
                return Humidity > 0 ? true : false;
            }
        }
        public bool IsTemperatureAccepted
        {
            get
            {
                return Temperature > 0 ? true : false;
            }
        }
        public bool IsCalibrated { get; set; }
    }
}
