using System.Text.RegularExpressions;

namespace SDKs.Dji;

/// <summary>
/// 大疆 R-JPEG
/// </summary>
public sealed class DjiRImage : IDisposable
{
    IntPtr _ph = IntPtr.Zero;
    decimal[,] mData = null;

    /// <summary>
    /// 图像宽度
    /// </summary>
    public int Width { get; private set; }
    /// <summary>
    /// 图像高度
    /// </summary>
    public int Height { get; private set; }
    /// <summary>
    /// 距离。1~25 米
    /// </summary>
    public decimal Distance { get;private set; }
    /// <summary>
    /// 相对湿度。20~100
    /// </summary>
    public int Humidity { get; private set; }
    /// <summary>
    /// 发射率。0.10^1.00
    /// </summary>
    public decimal Emissivity { get; private set; }
    /// <summary>
    /// 反射温度。-40.0℃~500.0℃
    /// </summary>
    public decimal Reflection { get; private set; }
    /// <summary>
    /// 图像文件大小
    /// </summary>
    public int Size { get; private set; }
    /// <summary>
    /// Rdf:Description 属性
    /// </summary>
    public RdfInfo Rdfs { get; set; }
    /// <summary>
    /// 图像 EXIF 信息
    /// </summary>
    public RExifInfo Exifs { get; set; }

    private DjiRImage()
    {
    }


    /// <summary>
    /// 从指定文件流创建大疆热红外 R-JPEG 图片
    /// </summary>
    /// <param name="filename">R-JPEG 文件名称</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static DjiRImage FromFile(string filename)
    {
        if (filename == null)
            throw new ArgumentNullException("filename");
        if (!System.IO.File.Exists(filename))
            throw new FileNotFoundException(filename);

        using (var stream = System.IO.File.OpenRead(filename))
            return FromStream(stream);
    }
    /// <summary>
    /// 从指定文件流创建大疆热红外 R-JPEG 图片
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public static DjiRImage FromStream(Stream stream)
    {
        if (stream == null || stream == Stream.Null)
            throw new ArgumentNullException(nameof(stream));

        int len = (int)stream.Length;
        byte[] buffer = new byte[len];
        stream.Read(buffer, 0, buffer.Length);
        var img = new DjiRImage();
        int code = img.Load(buffer);
        if (code == 0)
        {
            img.Rdfs = GetRdf(System.Text.Encoding.ASCII.GetString(buffer));
            stream.Position = 0;
            img.Exifs = GetExif(stream);
            return img;
        }
        img.Dispose();
        throw new InvalidDataException(((dirp_ret_code_e)code).ToString());
    }

    /// <summary>
    /// 从指定文件字节数组中创建大疆热红外 R-JPEG 图片
    /// </summary>
    /// <param name="bytes">文件字节数组</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static DjiRImage FromBytes(byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));

        var img = new DjiRImage();
        int code = img.Load(bytes);
        if (code == 0)
        {
            img.Rdfs = GetRdf(System.Text.Encoding.ASCII.GetString(bytes));
            using (var ms = new MemoryStream(bytes))
                img.Exifs = GetExif(ms);
            return img;
        }
        img.Dispose();
        throw new InvalidDataException(((dirp_ret_code_e)code).ToString());
    }
    int Load(byte[] rjpgData)
    {
        Size = rjpgData.Length;
        int code = TSDK.dirp_create_from_rjpeg(rjpgData, rjpgData.Length, ref _ph);
        if (code == 0)
        {
            dirp_resolution_t res = new dirp_resolution_t();
            TSDK.dirp_get_rjpeg_resolution(_ph, ref res);
            Width = res.width;
            Height = res.height;

            var mp = new dirp_measurement_params_t();
            TSDK.dirp_get_measurement_params(_ph, ref mp);
            Distance = Math.Round(Convert.ToDecimal(mp.distance), 2);
            Humidity = Convert.ToInt32(mp.humidity);
            Reflection = Math.Round(Convert.ToDecimal(mp.reflection), 1);
            Emissivity = Math.Round(Convert.ToDecimal(mp.emissivity), 2);

            int size = res.width * res.height * 2;
            byte[] buffer = new byte[size];
            TSDK.dirp_measure(_ph, buffer, size);
            TSDK.dirp_destroy(_ph);
            _ph = IntPtr.Zero;
            mData = Cast(buffer, res.width, res.height);
        }
        return code;
    }
    /// <summary>
    /// 获取整张图片区域温度
    /// </summary>
    /// <returns></returns>
    public TArea GetTemp()
    {
        return GetTempRect(0, 0, Width - 1, Height - 1).Value;
    }
    /// <summary>
    /// 获取图片指定位置的温度
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public decimal? GetTempP(int x, int y)
    {
        if (mData == null || mData.Length == 0)
            return null;
        if (x < 0 || x > Width - 1 || y < 0 || y > Height - 1)
            return null;

        return mData[x, y];
    }
    /// <summary>
    /// 获取图片指定线路上的区域温度
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public TArea? GetTempLine(int x1, int y1, int x2, int y2)
    {
        if (mData == null || mData.Length == 0)
            return null;
        if (x1 < 0 || x1 > Width - 1 || x2 < 0 || x2 > Width - 1)
            return null;
        if (y1 < 0 || y1 > Height - 1 || y2 < 0 || y2 > Height - 1)
            return null;

        var result = new TArea();
        int xoffset = x2 - x1;
        int yoffset = y2 - y1;
        int minx = x1 < x2 ? x1 : x2;
        int maxx = x1 < x2 ? x2 : x1;
        int miny = y1 < y2 ? y1 : y2;
        int maxy = y1 < y2 ? y2 : y1;
        int x = x1 < x2 ? x1 : x2;
        int y = x1 < x2 ? y1 : y2;
        decimal temp = mData[x, y];
        result.MinTemp = temp;
        result.MinX = x;
        result.MinY = y;
        result.MaxTemp = temp;
        result.MaxX = x;
        result.MaxY = y;
        result.AvgTemp = temp;

        if (xoffset == 0 && yoffset == 0)
            return result;

        List<decimal> tempArray = new List<decimal>();
        if (xoffset == 0)
        {
            for (int i = miny; i <= maxy; i++)
            {
                temp = mData[x, i];
                tempArray.Add(temp);

                if (temp < result.MinTemp)
                {
                    result.MinTemp = temp;
                    result.MinY = i;
                }
                if (temp > result.MaxTemp)
                {
                    result.MaxTemp = temp;
                    result.MaxY = i;
                }
            }
            result.AvgTemp = Math.Round(tempArray.Average(), 2);
            return result;
        }
        if (yoffset == 0)
        {
            for (int i = minx; i <= maxx; i++)
            {
                temp = mData[i, y];
                tempArray.Add(temp);

                if (temp < result.MinTemp)
                {
                    result.MinTemp = temp;
                    result.MinX = i;
                }
                if (temp > result.MaxTemp)
                {
                    result.MaxTemp = temp;
                    result.MaxX = i;
                }
            }
            result.AvgTemp = Math.Round(tempArray.Average(), 2);
            return result;
        }

        for (int i = minx; i <= maxx; i++)
        {
            for (int j = miny; j <= maxy; j++)
            {
                int dx1 = i - minx;
                int dy1 = j - miny;

                int dx2 = maxx - i;
                int dy2 = maxy - j;

                if (dx1 * dy2 != dx2 * dy1)
                    continue;

                temp = mData[i, y];
                tempArray.Add(temp);

                if (temp < result.MinTemp)
                {
                    result.MinTemp = temp;
                    result.MinX = i;
                    result.MinY = y;
                }
                if (temp > result.MaxTemp)
                {
                    result.MaxTemp = temp;
                    result.MaxX = i;
                    result.MaxY = y;
                }
            }
        }
        result.AvgTemp = Math.Round(tempArray.Average(), 2);
        return result;
    }
    /// <summary>
    /// 获取图像指定矩形范围内的区域温度
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public TArea? GetTempRect(int x, int y, int width, int height)
    {
        if (mData == null || mData.Length == 0)
            return null;

        if (width == 0 || height == 0)
            return GetTempLine(x, y, x + width, y + height);

        if (x < 0 || width < 0 || y < 0 || height < 0)
            return null;

        if (x + width > Width - 1 || y + height > Height - 1)
            return null;

        decimal temp = mData[x, y];
        var result = new TArea();
        result.MinTemp = temp;
        result.MinX = x;
        result.MinY = y;
        result.MaxTemp = temp;
        result.MaxX = x;
        result.MaxY = y;
        result.AvgTemp = temp;

        List<decimal> tempArray = new List<decimal>();

        int xoffset = x + width;
        int yoffset = y + height;
        for (int i = x; i < xoffset; i++)
        {
            for (int j = y; j < yoffset; j++)
            {
                temp = mData[i, j];
                tempArray.Add(temp);

                if (temp < result.MinTemp)
                {
                    result.MinTemp = temp;
                    result.MinX = i;
                    result.MinY = j;
                }
                if (temp > result.MaxTemp)
                {
                    result.MaxTemp = temp;
                    result.MaxX = i;
                    result.MaxY = j;
                }
            }
        }
        result.AvgTemp = Math.Round(tempArray.Average(), 2);
        return result;
    }
    static RdfInfo GetRdf(string text)
    {
        RdfInfo meta = new RdfInfo();
        string numpattern = "[\"][-+]*\\d+[.\\d+]*[\"]";
        var match = Regex.Match(text, "<rdf:Description[\\s\\S]+</rdf:Description>", RegexOptions.Multiline);
        if (!match.Success)
            return meta;


        var mc = Regex.Match(match.Value, $"drone-dji:Version=[\"][\\d]+[.\\d]*[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.Version = text;
        }
        mc = Regex.Match(match.Value, $"drone-dji:GpsStatus=[\"][\\w]+[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GpsStatus = text;
        }
        mc = Regex.Match(match.Value, $"drone-dji:GpsLatitude={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GpsLatitude = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GpsLongitude={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GpsLongitude = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GpsLongtitude={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GpsLongitude = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:AbsoluteAltitude={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.AbsoluteAltitude = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:RelativeAltitude={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.RelativeAltitude = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GimbalRollDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GimbalRollDegree = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GimbalYawDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GimbalYawDegree = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:GimbalPitchDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.GimbalPitchDegree = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightRollDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightRollDegree = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightYawDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightYawDegree = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightPitchDegree={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightPitchDegree = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightXSpeed={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightXSpeed = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightYSpeed={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightYSpeed = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:FlightZSpeed={numpattern}");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"').Trim('+');
            meta.FlightZSpeed = decimal.Parse(text);
        }
        mc = Regex.Match(match.Value, $"drone-dji:RtkFlag=[\"][\\d]+[\"]");
        if (mc.Success)
        {
            text = mc.Value.Split('=')[1].Trim('"');
            meta.RtkFlag = int.Parse(text);
        }
        return meta;
    }
    static RExifInfo GetExif(Stream stream)
    {
        string str;
        double[] dbArr;
        double db;
        DateTime dt;
        ushort ust;
        byte[] bArr;
        var exifs = new RExifInfo();
        using (var reader = new ExifLib.ExifReader(stream))
        {
            if (reader.GetTagValue(ExifLib.ExifTags.Make, out str))
                exifs.Make = str;
            if (reader.GetTagValue(ExifLib.ExifTags.Model, out str))
                exifs.Model = str;
            if (reader.GetTagValue(ExifLib.ExifTags.DateTime, out dt))
                exifs.DateTime = dt;
            if (reader.GetTagValue(ExifLib.ExifTags.FocalLength, out db))
                exifs.FocalLength = Convert.ToInt32(db);
            if (reader.GetTagValue(ExifLib.ExifTags.FocalLengthIn35mmFilm, out ust))
                exifs.FocalLengthIn35mmFilm = ust;
            if (reader.GetTagValue(ExifLib.ExifTags.GPSLatitude, out dbArr))
                exifs.GPSLatitude = Convert.ToDecimal(dbArr[0] + dbArr[1] / 60 + dbArr[2] / 3600);
            if (reader.GetTagValue(ExifLib.ExifTags.GPSLongitude, out dbArr))
                exifs.GPSLongitude = Convert.ToDecimal(dbArr[0] + dbArr[1] / 60 + dbArr[2] / 3600);
            if (reader.GetTagValue(ExifLib.ExifTags.GPSAltitude, out db))
                exifs.GPSAltitude = Convert.ToDecimal(db);
            if (reader.GetTagValue(ExifLib.ExifTags.ExifVersion, out bArr))
                exifs.ExifVersion = System.Text.Encoding.ASCII.GetString(bArr);
        }
        return exifs;
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

    public void Dispose()
    {
        if (_ph != IntPtr.Zero)
        {
            TSDK.dirp_destroy(_ph);
            _ph = IntPtr.Zero;
        }
    }
}