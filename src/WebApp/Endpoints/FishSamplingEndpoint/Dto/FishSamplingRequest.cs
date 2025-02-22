using FastEndpoints;

namespace WebApp.Endpoints.FishSamplingEndpoint.Dto;

public class FishSamplingRequest
{
    [QueryParam, BindFrom("start")]
    public long? Start { get; set; }

    [QueryParam, BindFrom("end")]
    public long? End { get; set; }
}