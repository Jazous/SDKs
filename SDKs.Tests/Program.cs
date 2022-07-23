using SDKs;
using SDKs.Dji;

byte[] data = System.IO.File.ReadAllBytes("DJI_0001_R.JPG");
using (var sdk = SDKs.Thermals.IRImageFactory.Create(data))
{
    if (sdk.Width > 0)
    {
        var dji = (DjiRImage)sdk;
        string text = sdk.Metas["drone-dji:GpsLatitude"];
        var temp = sdk.GetTemp();
        Console.WriteLine($"min-temp:{temp.Value.MinTemp}; max-temp:{temp.Value.MaxTemp}");
        Console.ReadKey();
    }
}
//ExifLib.ExifReader reader = new ExifLib.ExifReader("DJI_0001_R.JPG");

//Picturexif em = new Picturexif();

//Picturexif.Metadata m = em.GetEXIFMetaData("DJI_0001_R.JPG");//这里就是调用，传图片绝对路径 

//string exif = m.Ver.DisplayValue;

//string camera = m.CameraModel.DisplayValue;

//string model = m.CameraModel.DisplayValue;

//string aperture = m.Aperture.DisplayValue;

//string shutter = m.ShutterSpeed.DisplayValue;

//string sensitive = m.ExposureIndex.DisplayValue;

//Console.WriteLine(exif);
//Console.ReadKey();