namespace WebApp.Models;

public class FishSampling
{
    public string Id { get; set; } = null!;
    public long Timestamp { get; set; }
    public int FishId { get; set; }
    public int Weight { get; set; }
}