namespace AODB_Front.Config
{
    public static class AppConfig
    {
        
        public static readonly string BaseApiUrl = "https://localhost:5001/api";
        public static readonly string AuthenticationEndpoint = $"{BaseApiUrl}/authentication";
        
      
        public static readonly string FlightsEndpoint = $"{BaseApiUrl}/flights";
        public static readonly string AirportsEndpoint = $"{BaseApiUrl}/airports";
        public static readonly string AirlinesEndpoint = $"{BaseApiUrl}/airlines";
        public static readonly string AircraftEndpoint = $"{BaseApiUrl}/aircrafts";
        
        
        public static readonly int TimeoutSeconds = 30;
        public static readonly string ApplicationName = "AODB - Airport Operational Database";
    }
}
