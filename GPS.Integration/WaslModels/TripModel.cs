namespace GPS.Integration.WaslModels
{
    public class TripModel
    {
        public string tripNumber { get; set; }
        public double totalCost { get; set; }
        public string vehicleStatus { get; set; }
        public int distanceInMeters { get; set; }
        public int numberOfPassengers { get; set; }
        public string driverIdentityNumber { get; set; }
        public string sequenceNumber { get; set; }
        public double departureLatitude { get; set; }
        public double departureLongitude { get; set; }
        public string departedWhen { get; set; }
        public double expectedDestinationLongitude { get; set; }
        public double expectedDestinationLatitude { get; set; }
        public int paymentMethod { get; set; }
        public string activity { get; set; }
       
    }



    public class EndTripModel
    {
        public double totalCost { get; set; }
        public bool driverEmergencyButtonStatus { get; set; }
        public bool passengerEmergencyButtonStatus { get; set; }
        public string vehicleStatus { get; set; }

        public int distanceInMeters { get; set; }
        public int numberOfPassengers { get; set; }

        public int paymentMethod { get; set; }
        public string arrivedWhen { get; set; }

        public double actualDestinationLongitude { get; set; }
        public double actualDestinationLatitude { get; set; }
        
            
        public int customerRating { get; set; }


        public string activity { get; set; }


    }



    public class UpdateTripModel
    {
        public double currentTripCost { get; set; }

    }


}
