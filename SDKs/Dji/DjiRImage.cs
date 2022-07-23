using System.Text.RegularExpressions;

namespace SDKs.Dji;

/// <summary>
/// DJI Thermal SDK API.
/// </summary>
public sealed class DjiRImage : SDKs.Thermals.IRImage
{
    IntPtr _ph = IntPtr.Zero;
    decimal[,] _mData = null;
    int _width;
    int _height;
    int _size;
    DateTime _photoDate;
    string _format = null;
    string _cameraModel = null;
    string _cameraMaker = null;
    int? _humidity;
    static readonly string metaXmlpattern = "<rdf:Description[\\s\\S]+</rdf:Description>";
    static readonly string numpattern = "[\"][-+]*\\d+[.\\d+]*[\"]";

    protected override decimal[,] mData => _mData;
    public override int Width => _width;
    public override int Height => _height;
    public override DateTime PhotoDate => _photoDate;
    public override string Format => _format;
    public override string CameraModel => _cameraModel;
    public override string CameraMaker => _cameraMaker;
    public override int? Humidity => _humidity;
    public override int FileSize => _size;

    public RdfDescription Rdfs { get; set; }
    /// <summary>
    /// 解析红外图片，解析成功返回 null，否则返回错误信息。
    /// </summary>
    /// <param name="rjpgData"></param>
    /// <returns>返回 string.Empty 表示解析成功，非空表示错误信息。</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="DllNotFoundException"></exception>
    public override string Load(byte[] rjpgData)
    {
        if (rjpgData == null)
            throw new ArgumentNullException(nameof(rjpgData));
        if (_ph != IntPtr.Zero)
            throw new InvalidOperationException("rjpg data has been loaded.");

        _size = rjpgData.Length;

        int code = TSDK.dirp_create_from_rjpeg(rjpgData, rjpgData.Length, ref _ph);
        if (code != 0)
        {
            Dispose();
            return ((dirp_ret_code_e)code).ToString();
        }

        dirp_resolution_t res = new dirp_resolution_t();
        code = TSDK.dirp_get_rjpeg_resolution(_ph, ref res);
        if (code != 0)
        {
            Dispose();
            return ((dirp_ret_code_e)code).ToString();
        }

        _width = res.width;
        _height = res.height;

        dirp_measurement_params_t mp = new dirp_measurement_params_t();
        code = TSDK.dirp_get_measurement_params(_ph, ref mp);
        if (code == 0)
            _humidity = Convert.ToInt32(mp.humidity);

        int size = res.width * res.height * 2;
        byte[] buffer = new byte[size];
        code = TSDK.dirp_measure(_ph, buffer, size);
        if (code == 0)
        {
            Dispose();
            _mData = Cast(buffer, res.width, res.height);
            ProcessRdf(rjpgData);
            return null;
        }
        Dispose();
        return ((dirp_ret_code_e)code).ToString();
    }
    void ProcessRdf(byte[] rjpgData)
    {
        string text = System.Text.Encoding.ASCII.GetString(rjpgData);
        var match = Regex.Match(text, metaXmlpattern, RegexOptions.Multiline);
        if (!match.Success)
            return;

        RdfDescription meta = new RdfDescription();
        var mc = Regex.Match(match.Value, $"drone-dji:Version=[\"][\\d]+[.\\d]*[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.Version = text;
            Metas.Add("drone-dji:Version", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GpsStatus=[\"][\\w]+[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GpsStatus = text;
            Metas.Add("drone-dji:GpsStatus", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GpsLatitude={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GpsLatitude = decimal.Parse(text);
            Metas.Add("drone-dji:GpsLatitude", text);
        }

        mc = Regex.Match(match.Value, $"drone-dji:GpsLongitude={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GpsLongitude = decimal.Parse(text);
            Metas.Add("drone-dji:GpsLongitude", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GpsLongtitude={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GpsLongitude = decimal.Parse(text);
            Metas.Add("drone-dji:GpsLongitude", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:AbsoluteAltitude={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.AbsoluteAltitude = decimal.Parse(text);
            Metas.Add("drone-dji:AbsoluteAltitude", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:RelativeAltitude={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.RelativeAltitude = decimal.Parse(text);
            Metas.Add("drone-dji:RelativeAltitude", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GimbalRollDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GimbalRollDegree = decimal.Parse(text);
            Metas.Add("drone-dji:GimbalRollDegree", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GimbalYawDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GimbalYawDegree = decimal.Parse(text);
            Metas.Add("drone-dji:GimbalYawDegree", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GimbalPitchDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GimbalPitchDegree = decimal.Parse(text);
            Metas.Add("drone-dji:GimbalPitchDegree", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightRollDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightRollDegree = decimal.Parse(text);
            Metas.Add("drone-dji:FlightRollDegree", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightYawDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightYawDegree = decimal.Parse(text);
            Metas.Add("drone-dji:FlightYawDegree", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightPitchDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightPitchDegree = decimal.Parse(text);
            Metas.Add("drone-dji:FlightPitchDegree", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightXSpeed={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightXSpeed = decimal.Parse(text);
            Metas.Add("drone-dji:FlightXSpeed", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightYSpeed={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightYSpeed = decimal.Parse(text);
            Metas.Add("drone-dji:FlightYSpeed", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightZSpeed={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightZSpeed = decimal.Parse(text);
            Metas.Add("drone-dji:FlightZSpeed", text);
        }


        mc = Regex.Match(match.Value, $"drone-dji:CamReverse=[\"][\\d]+[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.CamReverse = int.Parse(text);
            Metas.Add("drone-dji:CamReverse", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GimbalReverse=[\"][\\d]+[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GimbalReverse = int.Parse(text);
            Metas.Add("drone-dji:GimbalReverse", text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:SelfData=[\"][\\w]+[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.SelfData = text;
            Metas.Add("drone-dji:SelfData", text);
        }

        mc = Regex.Match(match.Value, "xmp:CreateDate=[\"]\\d{4}-\\d{1,2}-\\d{1,2}[ T]{1}\\d{1,2}:\\d{1,2}:\\d{1,2}[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"');
            _photoDate = DateTime.Parse(text);
            Metas.Add("xmp:CreateDate", text);
        }

        mc = Regex.Match(match.Value, "dc:format=[\"][\\w]+[/\\w]*[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"');
            _format = text;
            Metas.Add("dc:format", text);
        }
        mc = Regex.Match(match.Value, "tiff:Model=[\"][\\w]+[-\\w]*[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"');
            _cameraModel = text;
            Metas.Add("tiff:Model", text);
        }
        mc = Regex.Match(match.Value, "tiff:Make=[\"][\\w]+[.\\w]*[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"');
            _cameraMaker = text;
            Metas.Add("tiff:Make", text);
        }
        this.Rdfs = meta;
    }

    decimal[,] Cast(byte[] rawData, int width, int height)
    {
        decimal[,] result = new decimal[width, height];
        int index = 0;
        byte[] temp = new byte[2];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                temp[0] = rawData[index];
                temp[1] = rawData[index + 1];
                index = index + 2;
                result[j, i] = BitConverter.ToInt16(temp, 0) * 0.1m;
            }
        }
        return result;
    }

    public override void Dispose()
    {
        if (_ph != IntPtr.Zero)
        {
            _mData = null;
            TSDK.dirp_destroy(_ph);
            _ph = IntPtr.Zero;
        }
    }
}