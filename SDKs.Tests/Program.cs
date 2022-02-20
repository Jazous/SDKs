// See https://aka.ms/new-console-template for more information
using SDKs.Dji;

Console.WriteLine("Hello, World!");

byte[] data = System.IO.File.ReadAllBytes("D://DJI_0001_R.JPG");
using (DjiThermal dji = new DjiThermal())
{
    if (dji.Analysis(data))
    {
        var tm = dji.GetTemp();
    }
}