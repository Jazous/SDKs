namespace SDKs.Thermals
{
    public sealed class IRImageFactory
    {
        public static IRImage Create(byte[] data)
        {
            var djiImage = new Dji.DjiRImage();
            djiImage.Load(data);
            return djiImage;
        }
    }
}