namespace WebApp.Endpoints.FishSamplingEndpoint.Dto;

public class FishSamplingDto
{
    public long Timestamp { get; set; }
    public int FishId { get; set; }
    public int Weight { get; set; }
}