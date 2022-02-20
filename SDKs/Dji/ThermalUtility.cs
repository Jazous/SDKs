namespace SDKs.Dji;

sealed class ThermalUtility
{
    public static float GetTemp(float[,] data, int maxWidth, int maxHeight, int x, int y)
    {
        if (data == null || data.Length == 0)
            return float.NaN;
        if (x < 0 || x > maxWidth - 1 || y < 0 || y > maxHeight - 1)
            return float.NaN;

        return data[x, y];
    }
    public static AreaTemp GetTemp(float[,] data, int maxWidth, int maxHeight,  int x1, int y1, int x2, int y2)
    {
        if (data == null || data.Length == 0)
            return null;

        int maxxIndex = maxWidth - 1;
        int maxyIndex = maxHeight - 1;
        if (x1 < 0 || x1 > maxxIndex || x2 < 0 || x2 > maxxIndex)
            return null;
        if (y1 < 0 || y1 > maxyIndex || y2 < 0 || y2 > maxyIndex)
            return null;

        AreaTemp result = new AreaTemp();
        int xoffset = x2 - x1;
        int yoffset = y2 - y1;
        int minx =  x1 < x2 ? x1 : x2;
        int maxx = x1 < x2 ? x2 : x1;
        int miny = y1 < y2 ? y1 : y2;
        int maxy = y1 < y2 ? y2 : y1;
        int x = x1 < x2 ? x1 : x2;
        int y = x1 < x2 ? y1 : y2;
        float temp = data[x, y];
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
                temp = data[x, i];
                tempArray.Add(Convert.ToDecimal(temp));

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
            result.AvgTemp = float.Parse(tempArray.Average().ToString("f2"));
            return result;
        }
        if (yoffset == 0)
        {
            for (int i = minx; i <= maxx; i++)
            {
                temp = data[i, y];
                tempArray.Add(Convert.ToDecimal(temp));

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
            result.AvgTemp = float.Parse(tempArray.Average().ToString("f2"));
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

                temp = data[i, y];
                tempArray.Add(Convert.ToDecimal(temp));

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
        result.AvgTemp = float.Parse(tempArray.Average().ToString("f2"));
        return result;
    }
    public static AreaTemp GetTempRect(float[,] data, int maxWidth, int maxHeight, int x, int y, int width, int height)
    {
        if (data == null || data.Length == 0)
            return null;
        if (x < 0 || x > maxWidth - 1 || y < 0 || y > maxHeight - 1)
            return null;
        if (width < 0 || width > maxWidth || x + width > maxWidth || height < 0 || height > maxHeight || y + height > maxHeight)
            return null;

        AreaTemp result = new AreaTemp();
        float temp = data[x, y];
        result.MinTemp = temp;
        result.MinX = x;
        result.MinY = y;
        result.MaxTemp = temp;
        result.MaxX = x;
        result.MaxY = y;
        result.AvgTemp = temp;

        if (width == 0 || height == 0)
            return result;

        List<decimal> tempArray = new List<decimal>();

        int xoffset = x + width;
        int yoffset = y + height;
        for (int i = x; i < xoffset; i++)
        {
            for (int j = y; j < yoffset; j++)
            {
                temp = data[i, j];
                tempArray.Add(Convert.ToDecimal(temp));

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
        result.AvgTemp = float.Parse(tempArray.Average().ToString("f2"));
        return result;
    }
}