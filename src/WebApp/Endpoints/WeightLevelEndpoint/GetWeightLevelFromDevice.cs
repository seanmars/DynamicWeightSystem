using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApp.Endpoints.WeightLevelEndpoint.Dto;
using WebApp.Service;

namespace WebApp.Endpoints.WeightLevelEndpoint;

public class GetWeightLevelFromDevice : Ep
    .NoReq
    .Res<Results<
        Ok<List<WeightLevelDto>>,
        BadRequest<string>
    >>
{
    private readonly DeviceService _deviceService;

    public GetWeightLevelFromDevice(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    public override void Configure()
    {
        Get("/weight-level-from-device");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<List<WeightLevelDto>>, BadRequest<string>>> ExecuteAsync(
        CancellationToken ct)
    {
        var deviceData = await _deviceService.GetWeightLevel(ct);
        var result = deviceData
            .Select(d => new WeightLevelDto()
            {
                Level = d.Level,
                LowerBound = d.LowerBound,
                UpperBound = d.UpperBound
            })
            .ToList();

        return TypedResults.Ok(result);
    }
}