namespace SDKs.Dji;

/// <summary>
/// DJI Thermal SDK API.
/// </summary>
public sealed class DjiThermal : IDisposable
{
    IntPtr _ph;
    float[,] _data;

    int _width;
    int _height;
    MeasureParams _measureParams;
    
    public int Width => _width;
    public int Height => _height;
    public MeasureParams MeasureParams { get => _measureParams; }

    public DjiThermal()
    {
        _ph = IntPtr.Zero;
    }
                  
    public static bool Recognize(string jpgFile)
    {
        return Recognize(System.IO.File.ReadAllBytes(jpgFile));
    }
    public static bool Recognize(byte[] jpgData)
    {
        IntPtr ph = IntPtr.Zero;
        if (TSDK.dirp_create_from_rjpeg(jpgData, jpgData.Length, ref ph) != 0)
            return false;
        TSDK.dirp_destroy(ph);
        ph = IntPtr.Zero;
        return true;
    }

    public bool Analysis(byte[] jpgData)
    {
        if (TSDK.dirp_create_from_rjpeg(jpgData, jpgData.Length, ref _ph) != 0)
            return false;

        dirp_resolution_t res = new dirp_resolution_t();
        if (TSDK.dirp_get_rjpeg_resolution(_ph, ref res) == 0)
        {
            if (res.width > 0 && res.height > 0)
            {
                this._width = res.width;
                this._height = res.height;

                TSDK.dirp_get_measurement_params(_ph, ref _measureParams);
                
                int size = res.width * res.height * 2;
                byte[] buffer = new byte[size];
                if (TSDK.dirp_measure(_ph, buffer, size) == 0)
                {
                    TSDK.dirp_destroy(_ph);
                    _ph = IntPtr.Zero;
                    _data = Cast(buffer, res.width, res.height);
                    return true;
                }
            }
        }
        if (_ph != IntPtr.Zero)
        {
            TSDK.dirp_destroy(_ph);
            _ph = IntPtr.Zero;
        }
        return false;
    }
    float[,] Cast(byte[] rawData, int width, int height)
    {
        float[,] result = new float[width, height];
        int index = 0;
        byte[] temp = new byte[2];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                temp[0] = rawData[index];
                temp[1] = rawData[index + 1];
                index = index + 2;
                result[j, i] = BitConverter.ToInt16(temp, 0) * 0.1f;
            }
        }
        return result;
    }
    public AreaTemp GetTemp()
    {
        return ThermalUtility.GetTempRect(_data, _width, _height, 0, 0, this._width, this._height);
    }
    public float GetTemp(int x, int y)
    {
        return ThermalUtility.GetTemp(this._data, this._width, this._height, x, y);
    }
    public AreaTemp GetTemp(int x1, int y1, int x2, int y2)
    {
        return ThermalUtility.GetTemp(this._data, this._width, this._height, x1, y1, x2, y2);
    }
    public AreaTemp GetTempRect(int x, int y, int width, int height)
    {
        return ThermalUtility.GetTempRect(_data, _width, _height, x, y, width, height);
    }
    public void Dispose()
    {
        if (_ph != IntPtr.Zero)
        {
            TSDK.dirp_destroy(_ph);
            _ph = IntPtr.Zero;
        }
    }
}