# SDKs

## Dji.DjiThermal
Dji thermal SDK Download Url： https://www.dji.com/cn/downloads/softwares/dji-thermal-sdk

```c#
byte[] data = System.IO.File.ReadAllBytes("D://DJI_0001_R.JPG");
using (var sdk = new SDKs.Dji.DjiThermal())
{
    if (sdk.Analysis(data))
    {
        var temp = sdk.GetTemp();
        Console.WriteLine($"min-temp:{temp.MinTemp}; max-temp:{temp.MaxTemp}");
    }
}
```

