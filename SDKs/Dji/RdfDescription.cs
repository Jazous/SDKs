namespace SDKs.Dji
{
    public struct RdfDescription
    {
        /// <summary>
        /// drone-dji:Version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// drone-dji:GpsStatus
        /// </summary>
        public string GpsStatus { get; set; }
        /// <summary>
        /// drone-dji:GpsLatitude
        /// </summary>
        public decimal GpsLatitude { get; set; }
        /// <summary>
        /// drone-dji:GpsLongitude
        /// </summary>
        public decimal GpsLongitude { get; set; }
        /// <summary>
        /// drone-dji:AbsoluteAltitude
        /// </summary>
        public decimal AbsoluteAltitude { get; set; }
        /// <summary>
        /// drone-dji:RelativeAltitude
        /// </summary>
        public decimal RelativeAltitude { get; set; }
        /// <summary>
        /// drone-dji:GimbalRollDegree
        /// </summary>
        public decimal GimbalRollDegree { get; set; }
        /// <summary>
        /// drone-dji:GimbalYawDegree
        /// </summary>
        public decimal GimbalYawDegree { get; set; }
        /// <summary>
        ///  drone-dji:GimbalPitchDegree
        /// </summary>
        public decimal GimbalPitchDegree { get; set; }
        /// <summary>
        /// drone-dji:FlightRollDegree
        /// </summary>
        public decimal FlightRollDegree { get; set; }
        /// <summary>
        /// drone-dji:FlightYawDegree
        /// </summary>
        public decimal FlightYawDegree { get; set; }
        /// <summary>
        /// drone-dji:FlightPitchDegree
        /// </summary>
        public decimal FlightPitchDegree { get; set; }
        /// <summary>
        /// drone-dji:FlightXSpeed
        /// </summary>
        public decimal FlightXSpeed { get; set; }
        /// <summary>
        /// drone-dji:FlightYSpeed
        /// </summary>
        public decimal FlightYSpeed { get; set; }
        /// <summary>
        /// drone-dji:FlightZSpeed
        /// </summary>
        public decimal FlightZSpeed { get; set; }
        /// <summary>
        /// drone-dji:CamReverse
        /// </summary>
        public int CamReverse { get; set; }
        /// <summary>
        /// drone-dji:GimbalReverse
        /// </summary>
        public int GimbalReverse { get; set; }
        /// <summary>
        /// drone-dji:SelfData
        /// </summary>
        public string SelfData { get; set; }
    }
}
