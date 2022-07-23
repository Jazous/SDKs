using SDKs.Thermals;

namespace SDKs.Dji
{
    struct dirp_measurement_params_range_t
    {
        /// <summary>
        /// 距离。[1^25] 米
        /// </summary>
        public KeyValuePair<float, float> distance;
        /// <summary>
        /// 空气湿度。[20^100]%
        /// </summary>
        public KeyValuePair<float, float> humidity;
        /// <summary>
        /// 发射率。[0.10^1.00]
        /// </summary>
        public KeyValuePair<float, float> emissivity;
        /// <summary>
        /// 反射温度。[-40.0~500.0]
        /// </summary>
        public KeyValuePair<float, float> reflection;
    }
}