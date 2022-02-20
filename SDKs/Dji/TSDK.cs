namespace SDKs.Dji;

static class TSDK
{
    [System.Runtime.InteropServices.DllImport("libdirp.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public extern static int dirp_create_from_rjpeg(byte[] data, int size, ref IntPtr ph);

    [System.Runtime.InteropServices.DllImport("libdirp.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public extern static int dirp_get_rjpeg_version(IntPtr h, ref dirp_rjpeg_version_t version);

    [System.Runtime.InteropServices.DllImport("libdirp.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public extern static int dirp_get_rjpeg_resolution(IntPtr h, ref dirp_resolution_t resolution);

    [System.Runtime.InteropServices.DllImport("libdirp.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public extern static int dirp_get_original_raw(IntPtr h, byte[] raw_image, int size);

    [System.Runtime.InteropServices.DllImport("libdirp.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public extern static int dirp_measure(IntPtr h, byte[] temp_image, int size);

    [System.Runtime.InteropServices.DllImport("libdirp.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public extern static int dirp_measure_ex(IntPtr h, byte[] temp_image, int size);

    [System.Runtime.InteropServices.DllImport("libdirp.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public extern static int dirp_get_measurement_params(IntPtr h, ref MeasureParams measurement_params);

    [System.Runtime.InteropServices.DllImport("libdirp.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public extern static int dirp_destroy(IntPtr h);
}

struct dirp_resolution_t
{
    /// <summary>
    /// Horizontal size
    /// </summary>
    public int width;
    /// <summary>
    /// Vertical size
    /// </summary>
    public int height;
}
struct dirp_rjpeg_version_t
{
    /// <summary>
    /// Version number of the opened R-JPEG itself.
    /// </summary>
    int rjpeg;
    /// <summary>
    /// Version number of the header data in R-JPEG
    /// </summary>
    int header;
    /// <summary>
    /// Version number of the curve LUT data in R-JPEG
    /// </summary>
    int curve;
}
/// <summary>
/// Temperature measurement parameters.
/// </summary>
public struct MeasureParams
{
    /// <summary>
    /// 距离。[1^25] 米
    /// </summary>
    public float Distance;
    /// <summary>
    /// 空气湿度。[20^100]%
    /// </summary>
    public float Humidity;
    /// <summary>
    /// 发射率。[0.10^1.00]
    /// </summary>
    public float Emissivity;
    /// <summary>
    /// 反射温度。[-40.0~500.0]
    /// </summary>
    public float Reflection;
}