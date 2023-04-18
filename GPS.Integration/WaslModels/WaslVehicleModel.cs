namespace GPS.Integration.WaslModels
{
    public class WaslVehicleModel
    {
        public string SequenceNumber { get; set; }

        public WaslPlate VehiclePlate { get; set; }

        public int PlateType { get; set; }

        public string IMEINumber { get; set; }

        public string Activity { get; set; }        

        public int? NumberOfSeats { get; set; } = 4;

        public string ColorCode { get; set; } = "Pantone 3278 C";

        public string StoringCategory { get; set; }
    }
}
