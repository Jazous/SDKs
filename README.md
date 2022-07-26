# SDKs

## Dji.DjiThermal
Dji thermal SDK Download Url： https://www.dji.com/cn/downloads/softwares/dji-thermal-sdk

```c#
byte[] data = System.IO.File.ReadAllBytes("1.JPG");
using (var img = DjiRImage.FromBytes(data))
{
    var tmp = img.GetTemp();
    Console.WriteLine($"制 作 商:{img.Exifs.Make}；\t\t\t 型号:{img.Exifs.Model}");
    Console.WriteLine($"文件大小:{img.Size}字节; \t\t 拍摄日期:{img.Exifs.DateTime}");
    Console.WriteLine($"最低温度:{tmp.MinTemp}; \t\t\t 最高温度:{tmp.MaxTemp}; \t\t 平均温度:{tmp.AvgTemp}");
    Console.WriteLine($"无人机偏航角:{img.Rdfs.FlightYawDegree}; \t\t 无人机俯仰角:{img.Rdfs.FlightPitchDegree}; \t\t 无人机翻滚角:{img.Rdfs.FlightRollDegree}");
    Console.WriteLine($"云台偏航角:{img.Rdfs.GimbalYawDegree}; \t\t 云台俯仰角:{img.Rdfs.GimbalPitchDegree}; \t\t 云台翻滚角:{img.Rdfs.GimbalRollDegree}");
    Console.WriteLine($"经度:{img.Rdfs.GpsLongitude}; \t\t 纬度:{img.Rdfs.GpsLatitude}; \t\t 海拔:{img.Rdfs.AbsoluteAltitude}");
}
Console.ReadKey();
```