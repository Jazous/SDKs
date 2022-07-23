namespace SDKs.Thermals;

public struct TArea
{
    public decimal MinTemp { get; set; }
    public int MinX { get; set; }
    public int MinY { get; set; }
    public decimal MaxTemp { get; set; }
    public int MaxX { get; set; }
    public int MaxY { get; set; }
    public decimal AvgTemp { get; set; }
}