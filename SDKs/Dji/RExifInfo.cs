namespace SDKs.Dji
{
    /// <summary>
    /// 大疆 R-JPEG 热红外照片 EXIF 信息
    /// </summary>
    /// <remarks>经纬度坐标为 WGS-84 标准。</remarks>
    public struct RExifInfo
    {
        /// <summary>
        /// 制造商
        /// </summary>
        public string Make { get; set; }
        /// <summary>
        /// 相机型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 拍摄时间
        /// </summary>
        public DateTime? DateTime { get; set; }
        /// <summary>
        /// 焦距。单位：毫米
        /// </summary>
        public int FocalLength { get; set; }
        /// <summary>
        /// 35mm 焦距
        /// </summary>
        public int FocalLengthIn35mmFilm { get; set; }
        /// <summary>
        /// 纬度。WGS-84
        /// </summary>
        public decimal? GPSLatitude { get; set; }
        /// <summary>
        /// 经度。WGS-84
        /// </summary>
        public decimal? GPSLongitude { get; set; }
        /// <summary>
        /// 高度。WGS-84，单位：米
        /// </summary>
        public decimal? GPSAltitude { get; set; }
        /// <summary>
        /// EXIF 版本
        /// </summary>
        public string ExifVersion { get; set; }
    }
}