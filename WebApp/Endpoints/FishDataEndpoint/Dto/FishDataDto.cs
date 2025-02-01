namespace WebApp.Endpoints.FishDataEndpoint.Dto;

public class FishDataDto
{
    public string? Id { get; set; } = string.Empty;
    public required string FishCode { get; set; }
    public required string Name { get; set; }
}