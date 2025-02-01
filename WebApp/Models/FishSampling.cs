namespace WebApp.Models;

public class FishSampling
{
    public required string Id { get; set; } = Guid.CreateVersion7().ToString();
    public required long Timestamp { get; set; }
    public required string FishCode { get; set; }
    public required int Weight { get; set; }
}