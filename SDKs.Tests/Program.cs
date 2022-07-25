using SDKs;
using SDKs.Dji;
using System.Diagnostics;

byte[] data = System.IO.File.ReadAllBytes("1.JPG");
var fs = File.OpenRead("1.JPG");

Stopwatch watch = new Stopwatch();
watch.Start();
using (var img = DjiRImage.FromBytes(data))
{
    watch.Stop();
    string text = img.Rdfs.RtkFlag.ToString();
    var temp = img.GetTemp();
    Console.WriteLine($"min-temp:{temp.Value.MinTemp}; max-temp:{temp.Value.MaxTemp}");
}
Console.WriteLine($"ElapsedMilliseconds={watch.ElapsedMilliseconds}");
Console.ReadKey();
