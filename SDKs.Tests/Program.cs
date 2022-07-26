using SDKs;
using SDKs.Dji;
using System.Diagnostics;
using System.Text.RegularExpressions;

//int idxmax = 0;
//int idxcmax = 0;
//for (int i = 1; i < 8; i++)
//{
//    string pname = i + ".JPG";
//    byte[] data = System.IO.File.ReadAllBytes(pname);
//    //Stopwatch watch = new Stopwatch();
//    //watch.Start();
//    using (var img = DjiRImage.FromBytes(data))
//    {
//        if (img.idx > idxmax)
//            idxmax = img.idx;
//        if (img.idxcount > idxcmax)
//            idxcmax = img.idxcount;
//        var tmp = img.GetTemp();
//        string text = $"{i}：idx={img.idx,5}\t idxc={img.idxcount,4}\t size:{img.Size}\t min:{tmp.MinTemp}\t max;{tmp.MaxTemp}\t dt:{img.Exifs.DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}\t model:{new string(img.Exifs.Model.Take(6).ToArray())}";
//        Console.WriteLine(text);
//        //var temp = img.GetTemp();
//        //Console.WriteLine($"min-temp:{temp.MinTemp}; max-temp:{temp.MaxTemp}");
//    }
//}
//Console.ReadKey();