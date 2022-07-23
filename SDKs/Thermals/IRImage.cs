namespace SDKs.Thermals;

public abstract class IRImage : IDisposable
{
    protected abstract decimal[,] mData { get; }
    public abstract int Width { get; }
    public abstract int Height { get; }
    public abstract DateTime PhotoDate { get; }
    public abstract string Format { get; }
    public abstract string CameraModel { get; }
    public abstract string CameraMaker { get; }
    public abstract int? Humidity { get; }
    public abstract int FileSize { get; }
    public System.Collections.Specialized.NameValueCollection Metas { get; }
    public System.Collections.Specialized.NameValueCollection Exifs { get; }

    public IRImage()
    {
        Metas = new System.Collections.Specialized.NameValueCollection();
        Exifs = new System.Collections.Specialized.NameValueCollection();
    }


    public string Load(Stream inputStream)
    {
        if (inputStream == null)
            throw new ArgumentNullException("inputStream");

        byte[] buffer = new byte[inputStream.Length];
        inputStream.Seek(0, SeekOrigin.Begin);
        inputStream.Read(buffer, 0, buffer.Length);
        return Load(buffer);
    }
    public string Load(string path)
    {
        return Load(System.IO.File.ReadAllBytes(path));
    }
    /// <summary>
    /// 解析红外图片，解析成功返回 null，否则返回错误信息。
    /// </summary>
    /// <param name="rjpgData"></param>
    /// <returns>返回 string.Empty 表示解析成功，非空表示错误信息。</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="DllNotFoundException"></exception>
    public abstract string Load(byte[] rjpgData);

    public TArea? GetTemp()
    {
        return GetTempRect(0, 0, Width - 1, Height - 1);
    }
    public decimal? GetTemp(int x, int y)
    {
        if (mData == null || mData.Length == 0)
            return null;
        if (x < 0 || x > Width - 1 || y < 0 || y > Height - 1)
            return null;

        return mData[x, y];
    }
    public TArea? GetTemp(int x1, int y1, int x2, int y2)
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
    public TArea? GetTempRect(int x, int y, int width, int height)
    {
        if (mData == null || mData.Length == 0)
            return null;

        if (width == 0 || height == 0)
            return GetTemp(x, y, x + width, y + height);

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

    public abstract void Dispose();
}