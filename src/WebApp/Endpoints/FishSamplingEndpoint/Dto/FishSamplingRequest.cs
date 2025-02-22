using FastEndpoints;

namespace WebApp.Endpoints.FishSamplingEndpoint.Dto;

public class FishSamplingRequest
{
    [QueryParam, BindFrom("start")]
    public long? Start { get; set; }

    [QueryParam, BindFrom("end")]
    public long? End { get; set; }

    [QueryParam, BindFrom("fish")]
    public List<string>? FishCodes { get; set; }
}