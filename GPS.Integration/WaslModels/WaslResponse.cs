namespace GPS.Integration.WaslModels
{
    public class WaslResponse
    {
        public bool Success { get; set; }
        public string ResultCode { get; set; }
        public WaslResult Result { get; set; }

    }
}
